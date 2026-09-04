using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.LocalAuth;

namespace Piranha.Manager.LocalAuth.Areas.Manager.Pages.IdentityExtensions
{
    /// <summary>
    /// Replaces Piranha's own manager logout page.
    ///
    /// It exists because taking over the login page breaks Piranha's logout. IdentityRoutingConvention
    /// unmaps /Areas/Manager/Pages/Login.cshtml by removing its route selector, which leaves that page
    /// present but routeless - and Piranha's LogoutModel finishes with RedirectToPage("Login"). The page
    /// NAME still resolves, so there is no error; there is simply no endpoint to generate a URL from, and
    /// link generation degrades to the current path plus a query value, landing the user on /?page=%2FLogin.
    ///
    /// So this signs out the same way and redirects to a literal URL. LocalRedirect, deliberately, not
    /// RedirectToPage - the whole failure above came from generating a URL by page name.
    /// </summary>
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly ISecurity _service;

        public LogoutModel(ISecurity service)
        {
            _service = service;
        }

        public async Task<IActionResult> OnGet()
        {
            await _service.SignOut(HttpContext);

            return LocalRedirect("~/manager/login");
        }
    }
}
