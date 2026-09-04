/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.Localization;
using SoundInTheory.Piranha.Identity.Extensions.Model;
using SoundInTheory.Piranha.Identity.Extensions.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Piranha.Manager.LocalAuth.Areas.Manager.Pages.IdentityExtensions
{
    /// <summary>
    /// View model for the login page.
    /// </summary>
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ISecurity _service;
        private readonly ManagerLocalizer _localizer;
        private readonly LoginThemeService _loginTheme;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The current security service</param>
        /// <param name="localizer">The manager localizer</param>
        public LoginModel(ISecurity service, ManagerLocalizer localizer, LoginThemeService loginTheme)
        {
            _service = service;
            _localizer = localizer;
            _loginTheme = loginTheme;
        }

        /// <summary>
        /// Gets/sets the model for binding form data.
        /// </summary>
        /// <value></value>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// Gets/sets the optional return url after successful
        /// authorization.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets/sets the possible error message to be returned
        /// after failed authorization.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// A one-shot confirmation handed over by another page - currently ResetPassword after a successful
        /// reset. The property name is the TempData key, so it has to match the property that sets it.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }


        public ILoginTheme Theme { get; set; }

        /// <summary>
        /// Model for form data.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Gets/sets the user name.
            /// </summary>
            [Required]
            public string Username { get; set; }

            /// <summary>
            /// Gets/sets the password.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        /// Gets the login page.
        /// </summary>
        /// <param name="returnUrl">The optional return url</param>
        public void OnGet(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            Theme = _loginTheme.GetLoginTheme();

            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// Handles authorization after a post.
        /// </summary>
        /// <param name="returnUrl">The optional return url</param>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            await _service.SignOut(HttpContext);
            Theme = _loginTheme.GetLoginTheme();

            if (!ModelState.IsValid || (await _service.SignIn(HttpContext, Input.Username, Input.Password)) != LoginResult.Succeeded)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, _localizer.General["Username and/or password are incorrect."].Value);
                return Page();
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect($"~/manager/login/auth?returnUrl={returnUrl}");
            }
            return LocalRedirect("~/manager/login/auth");
        }
    }
}