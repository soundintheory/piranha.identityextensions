using SoundInTheory.Piranha.Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Identity.Extensions.EmailViewModels
{
    public class ManagerForgotPasswordEmailViewModel : EmailViewModel
    {
        public string Name { get; set; }
        public string Link { get; set;}

    }
}
