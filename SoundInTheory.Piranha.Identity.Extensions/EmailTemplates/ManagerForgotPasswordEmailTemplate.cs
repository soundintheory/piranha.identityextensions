using Mjml.Net.Helpers;
using SoundInTheory.Piranha.Emails.Attributes;
using SoundInTheory.Piranha.Emails.Models;
using SoundInTheory.Piranha.Identity.Extensions.EmailViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Identity.Extensions.EmailTemplates
{


    [EmailTemplate(enabled: true)]
    public class ManagerForgotPasswordEmailTemplate : EmailTemplateInfo
    {
        public new const string Key = "ManagerForgotPasswordEmail";

        public ManagerForgotPasswordEmailTemplate()
        {
            Title = "Forgot Password Email (Manager)";

            SampleData = new ManagerForgotPasswordEmailViewModel()
            {
                Link = "https://example.com",
                Name = "Gunnar Gunnarson"
            };
        }
    }
}
