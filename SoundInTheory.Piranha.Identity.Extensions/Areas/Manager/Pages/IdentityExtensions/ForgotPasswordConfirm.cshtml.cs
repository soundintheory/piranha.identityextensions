using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.LocalAuth;
using SoundInTheory.Piranha.Identity.Extensions.Model;
using SoundInTheory.Piranha.Identity.Extensions.Services;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Manager.LocalAuth.Areas.Manager.Pages.IdentityExtensions
{
    [AllowAnonymous]
    public class ForgotPasswordConfirm : PageModel
    {
        private readonly ISecurity _service;
        private readonly LoginThemeService _loginTheme;

        public ForgotPasswordConfirm(ISecurity service, LoginThemeService loginTheme)
        {
            _service = service;
            _loginTheme = loginTheme;
        }
        

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


        }
    }
}
