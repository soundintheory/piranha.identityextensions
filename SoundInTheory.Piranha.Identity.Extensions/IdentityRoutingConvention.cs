using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Identity.Extensions
{
    public class IdentityRoutingConvention : IPageRouteModelConvention
    {
        public void Apply(PageRouteModel model)
        {
            // Unmap Piranha's own login page and put ours on the URL it freed up.
            if (model.RelativePath == "/Areas/Manager/Pages/Login.cshtml")
            {
                model.Selectors.RemoveAt(0);
            }

            if (model.RelativePath == "/Areas/Manager/Pages/IdentityExtensions/Login.cshtml")
            {
                model.Selectors[0].AttributeRouteModel.Template = "manager/login";
            }

            // Logout has to be swapped as well, and NOT because we want to change it.
            //
            // Removing the login page's selector above leaves that page present but routeless. Piranha's
            // LogoutModel ends with RedirectToPage("Login"): the page name still resolves, so nothing errors,
            // but there is no endpoint left to generate a URL from and link generation degrades to the current
            // path plus a query value - dumping the user on /?page=%2FLogin instead of the login screen.
            //
            // Ours redirects to a literal URL instead, so it cannot fail the same way.
            if (model.RelativePath == "/Areas/Manager/Pages/Logout.cshtml")
            {
                model.Selectors.RemoveAt(0);
            }

            if (model.RelativePath == "/Areas/Manager/Pages/IdentityExtensions/Logout.cshtml")
            {
                model.Selectors[0].AttributeRouteModel.Template = "manager/logout";
            }
        }
    }
}
