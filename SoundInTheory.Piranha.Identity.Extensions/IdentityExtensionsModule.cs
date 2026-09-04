using Piranha;
using Piranha.Extend;
using Piranha.Manager;
using Piranha.Security;

namespace SoundInTheory.Piranha.Identity.Extensions
{
    public class IdentityExtensionsModule : IModule
    {
        private readonly List<PermissionItem> _permissions = new List<PermissionItem>
        {
           
        };

        /// <summary>
        /// Gets the module author
        /// </summary>
        public string Author => "Sound in Theory";

        /// <summary>
        /// Gets the module name
        /// </summary>
        public string Name => "Piranha Identity Extensions";

        /// <summary>
        /// Gets the module version
        /// </summary>
        public string Version => Utils.GetAssemblyVersion(GetType().Assembly);

        /// <summary>
        /// Gets the module description
        /// </summary>
        public string Description => "Identity and user extensions for Piranha";

        /// <summary>
        /// Gets the module package url
        /// </summary>
        public string PackageUrl => "";

        /// <summary>
        /// Gets the module icon url
        /// </summary>
        // Must match the RequestPath the embedded assets are served under in UseIdentityExtensions.
        public string IconUrl => "/manager/IdentityExtensions/piranha-logo.png";

        public void Init()
        {
        }
    }
}
