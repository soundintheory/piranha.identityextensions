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

            var template = await _emailTemplateResolver.ResolveAsync(ManagerForgotPasswordEmailTemplate.Key);

            var result = _email.Send(
                template.Template.EmailSenderKey,
                Input.Email,
                template.Template.SubjectLine,
                template.Template.Content,
                new ManagerForgotPasswordEmailViewModel()
                {
                    Link = Url.Action("reset-password", "manager", new { token, Input.Email }, Request.Scheme),
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
