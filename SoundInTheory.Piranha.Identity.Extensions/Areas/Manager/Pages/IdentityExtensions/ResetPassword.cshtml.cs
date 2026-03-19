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
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<Piranha.AspNetCore.Identity.Data.User> _service;
        private readonly LoginThemeService _loginTheme;
        private readonly GenericEmailHandler _email;
        private readonly EmailTemplateResolver _emailTemplateResolver;

        public ResetPasswordModel(
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
            public string ConfirmNewPassword { get; set;}

            [Required]
            public string NewPassword { get; set; }
        }

        /// <summary>
        /// Gets/sets the model for binding form data.
        /// </summary>
        /// <value></value>
        [BindProperty]
        public InputModel Input { get; set; }

        public ILoginTheme Theme { get; set; }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Token { get; set; }

        /// <summary>
        /// Gets/sets the possible error message to be returned
        /// after failed authorization.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet(string email, string token)
        {
            Theme = _loginTheme.GetLoginTheme();
            Email = email;
            Token = token;
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

        }

        public async Task<IActionResult> OnPost()
        {
            Theme = _loginTheme.GetLoginTheme();

            if (!ModelState.IsValid || Input.ConfirmNewPassword != Input.NewPassword)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Passwords are required.");
                return Page();
            }

            var usr = await _service.FindByEmailAsync(Email);


            if(usr == null)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Invalid reset password link.");
                return Page();
            }

            var roles = await _service.GetRolesAsync(usr);

            if (!roles.Contains("SysAdmin"))
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Invalid user.");
                return Page();
            }

            var pw = await _service.ResetPasswordAsync(usr, Token, Input.NewPassword);


            if (!pw.Succeeded)
            {
                return StatusCode(500, "Failed to send reset password");
            }

            return LocalRedirect("~/manager/login/auth");
        }
    }
}
