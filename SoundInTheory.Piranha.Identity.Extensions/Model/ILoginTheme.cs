using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Identity.Extensions.Model
{
    public interface ILoginTheme
    {
        public string Title { get; set; }
        public string BackgroundColor { get; set; }
        public string ButtonColor { get; set; }
        public string LogoPath { get; set; }
        public string BgPath { get; set; }
        public bool IncludeVersionNumber { get; set; }

    }
}
