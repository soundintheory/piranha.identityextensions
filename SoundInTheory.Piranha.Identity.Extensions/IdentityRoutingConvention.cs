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
            //Remove the old media manager
            if (model.RelativePath == "/Areas/Manager/Pages/Login.cshtml")
            {
                model.Selectors.RemoveAt(0);
            }

            if (model.RelativePath == "/Areas/Manager/Pages/IdentityExtensions/Login.cshtml")
            {
                model.Selectors[0].AttributeRouteModel.Template = "manager/login";
            }
        }
    }
}
