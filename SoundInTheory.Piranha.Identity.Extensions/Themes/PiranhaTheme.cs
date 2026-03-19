using SoundInTheory.Piranha.Identity.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Identity.Extensions.Themes
{
    public class PiranhaTheme : ILoginTheme
    {
        public string Title { get; set; } = "Piranha CMS";
        public string BackgroundColor { get; set; } = "#0C3C50";
        public string ButtonColor { get; set; } = "#007eaa";
        public string LogoPath { get; set; } = "/manager/assets/img/logo.png";
        public string BgPath { get; set; } = "/manager/assets/img/login.png";
        public bool IncludeVersionNumber { get; set; } = true;
    }
}
