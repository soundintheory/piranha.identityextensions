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

        /// <summary>
        /// Handed to the login page after a successful reset. The property name is the TempData key, so it
        /// must match the property on LoginModel that reads it.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// False once the link has been used, has expired, or never matched a user. The view hides the form
        /// entirely in that case - offering password boxes that cannot possibly work is what made a spent
        /// link look like it still worked.
        /// </summary>
        public bool LinkIsValid { get; set; } = true;

        private const string LinkNoLongerValid =
            "This password reset link is no longer valid - it has either expired or already been used. " +
            "Please request a new one.";

        public async Task<IActionResult> OnGet(string email, string token)
        {
            Theme = _loginTheme.GetLoginTheme();
            Email = email;
            Token = token;

            // Validate up front. OnGet used to render the form for ANY url, so a link that had already been
            // spent still produced a working-looking page and only failed on submit - as "Invalid Token",
            // which reads like a problem with the password that was just typed.
            LinkIsValid = await IsResetTokenValidAsync(email, token);

            if (!LinkIsValid)
            {
                ModelState.AddModelError(string.Empty, LinkNoLongerValid);
            }
            else if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            Theme = _loginTheme.GetLoginTheme();

            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Both password fields are required.");
                return Page();
            }

            if (Input.ConfirmNewPassword != Input.NewPassword)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "The passwords do not match.");
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
                // An expired/invalid token and a password failing the configured complexity rules both land
                // here, and they need completely different wording - Identity's own "Invalid token." tells a
                // user who simply chose a weak password nothing they can act on.
                ModelState.Clear();

                if (pw.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.InvalidToken)))
                {
                    LinkIsValid = false;
                    ModelState.AddModelError(string.Empty, LinkNoLongerValid);
                }
                else if (pw.Errors.Any(x => x.Code != null && x.Code.StartsWith("Password", StringComparison.Ordinal)))
                {
                    ModelState.AddModelError(string.Empty, $"Please enter a more complex password. {DescribePasswordRules()}");
                }
                else if (pw.Errors.Any())
                {
                    foreach (var error in pw.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The password could not be reset. Please request a new reset link.");
                }

                return Page();
            }

            StatusMessage = "Password reset successful. You can now login using your new password";

            // Straight to the login page, NOT /manager/login/auth - that is the post-sign-in finaliser, and
            // nobody is signed in at this point.
            return LocalRedirect("~/manager/login");
        }

        /// <summary>
        /// Pure check - does not consume the token. A reset token is bound to the user's security stamp, and
        /// a successful reset rolls that stamp, which is what makes a used link stop verifying.
        /// </summary>
        private async Task<bool> IsResetTokenValidAsync(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return false;
            }

            var usr = await _service.FindByEmailAsync(email);

            if (usr == null)
            {
                return false;
            }

            var roles = await _service.GetRolesAsync(usr);

            if (!roles.Contains("SysAdmin"))
            {
                return false;
            }

            return await _service.VerifyUserTokenAsync(
                usr,
                _service.Options.Tokens.PasswordResetTokenProvider,
                UserManager<Piranha.AspNetCore.Identity.Data.User>.ResetPasswordTokenPurpose,
                token
            );
        }

        /// <summary>
        /// Describes the CONFIGURED rules rather than a hardcoded sentence, so the message cannot drift from
        /// what the validator actually enforces if the policy is ever changed.
        /// </summary>
        private string DescribePasswordRules()
        {
            var policy = _service.Options.Password;
            var clauses = new List<string>();

            if (policy.RequiredLength > 0)
            {
                clauses.Add($"be at least {policy.RequiredLength} characters long");
            }

            var contains = new List<string>();

            if (policy.RequireDigit) contains.Add("a number");
            if (policy.RequireLowercase) contains.Add("a lower case letter");
            if (policy.RequireUppercase) contains.Add("an upper case letter");
            if (policy.RequireNonAlphanumeric) contains.Add("a symbol");
            if (policy.RequiredUniqueChars > 1) contains.Add($"at least {policy.RequiredUniqueChars} different characters");

            if (contains.Count > 0)
            {
                clauses.Add($"contain {JoinReadable(contains)}");
            }

            return clauses.Count == 0
                ? "Please choose a different password."
                : $"The password must {JoinReadable(clauses)}.";
        }

        private static string JoinReadable(List<string> parts)
        {
            if (parts.Count == 1)
            {
                return parts[0];
            }

            return $"{string.Join(", ", parts.Take(parts.Count - 1))} and {parts[parts.Count - 1]}";
        }
    }
}
