using Microsoft.Extensions.DependencyInjection;
using SoundInTheory.Piranha.Identity.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Identity.Extensions.Services
{
    public class LoginThemeService(
        IServiceProvider _services    
    )
    {
        public ILoginTheme GetLoginTheme()
        {
            //order by latest registered
            return _services.GetServices<ILoginTheme>().Reverse().First();
        }
    }
}
