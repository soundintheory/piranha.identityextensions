using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.LocalAuth;
using SoundInTheory.Piranha.Emails.Services;
using SoundInTheory.Piranha.Identity.Extensions.EmailTemplates;
using SoundInTheory.Piranha.Identity.Extensions.EmailViewModels;
using SoundInTheory.Piranha.Identity.Extensions.Model;
using SoundInTheory.Piranha.Identity.Extensions.Services;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Manager.LocalAuth.Areas.Manager.Pages.IdentityExtensions
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<Piranha.AspNetCore.Identity.Data.User> _service;
        private readonly LoginThemeService _loginTheme;
        private readonly GenericEmailHandler _email;
        private readonly EmailTemplateResolver _emailTemplateResolver;

        public ForgotPasswordModel(
            UserManager<Piranha.AspNetCore.Identity.Data.User> service, 
            LoginThemeService loginTheme, 
            GenericEmailHandler email, 
            EmailTemplateResolver emailTemplateResolver
        )
        {
            _service = service;
            _loginTheme = loginTheme;
            _email = email;
            _emailTemplateResolver = emailTemplateResolver;
        }

        public class InputModel
        {
            [Required]
            public string Email { get; set;}
        }

        /// <summary>
        /// Gets/sets the model for binding form data.
        /// </summary>
        /// <value></value>
        [BindProperty]
        public InputModel Input { get; set; }

        public ILoginTheme Theme { get; set; }

        /// <summary>
        /// Gets/sets the possible error message to be returned
        /// after failed authorization.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            Theme = _loginTheme.GetLoginTheme();

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

        }

        public async Task<IActionResult> OnPost()
        {
            Theme = _loginTheme.GetLoginTheme();

            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Email is required");
                return Page();
            }

            var usr = await _service.FindByEmailAsync(Input.Email);

            if(usr == null)
            {
                return Redirect("/manager/forgot-password-confirm");
            }

            var roles = await _service.GetRolesAsync(usr);

            if (!roles.Contains("SysAdmin"))
            {
                return Redirect("/manager/forgot-password-confirm");
            }

            var token = await _service.GeneratePasswordResetTokenAsync(usr);

            // ResetPassword is a Razor Page, not an MVC action, so it has to be generated with Url.Page.
            // Url.Action returns null when nothing matches, which sent the email with an empty link.
            var link = Url.Page(
                "/IdentityExtensions/ResetPassword",
                pageHandler: null,
                values: new { area = "Manager", email = Input.Email, token },
                protocol: Request.Scheme
            );

            if (string.IsNullOrEmpty(link))
            {
                // Fail loudly rather than send an email whose link goes nowhere.
                return StatusCode(500, "Could not generate the password reset link");
            }

            var template = await _emailTemplateResolver.ResolveAsync(ManagerForgotPasswordEmailTemplate.Key);

            // Existence is not enough. EmailTemplatesStartup auto-creates the row on boot with Content = ""
            // and SubjectLine left NULL, so an unauthored template gets this far and then dies inside
            // Handlebars.Compile(null) with an ArgumentNullException naming a parameter called 's'. Check the
            // fields that actually have to be there.
            if (template?.Template == null
                || string.IsNullOrWhiteSpace(template.Template.SubjectLine)
                || string.IsNullOrWhiteSpace(template.Template.Content))
            {
                return StatusCode(500,
                    $"Email template '{ManagerForgotPasswordEmailTemplate.Key}' has not been set up - " +
                    "it needs a subject line and content adding in the manager");
            }

            var result = _email.Send(
                template.Template.EmailSenderKey,
                Input.Email,
                template.Template.SubjectLine,
                template.Template.Content,
                new ManagerForgotPasswordEmailViewModel()
                {
                    Link = link,
                    Name = usr.UserName
                }
            );

            if (!result.Successful)
            {
                return StatusCode(500, "Failed to send reset email");
            }

            return Redirect("/manager/forgot-password-confirm");
        }
    }
}
