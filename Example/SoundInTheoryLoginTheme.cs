using SoundInTheory.Piranha.Identity.Extensions.Model;

namespace Example
{
    public class SoundInTheoryLoginTheme : ILoginTheme
    {
        public string Title { get; set; } = "Sound in Theory Admin";
        public string BackgroundColor { get; set; } = "#150536";
        public string ButtonColor { get; set; } = "#ea6e2a";
        public string LogoPath { get; set; } = "/assets/img/piranha-login-logo.png";
        public string BgPath { get; set; } = "/assets/img/piranha-login-bg.png";
        public bool IncludeVersionNumber { get; set; } = false;
    }
}
