using Piranha.Manager.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Identity.Extensions
{
    public static class LoginPageActions
    {
        public sealed class IdentityToolbarActions
        {
            public ActionList<ToolbarAction> Login { get; private set; } = new ActionList<ToolbarAction>()
            {
                new ToolbarAction()
                {
                    InternalId = "ForgotPassword",
                    ActionView = "Partial/Actions/_ForgotPassword"
                }
            };

            public static IdentityToolbarActions Toolbars { get; private set; } = new IdentityToolbarActions();
        }
    }
}
