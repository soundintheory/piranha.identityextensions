using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Piranha;
using Piranha.AspNetCore;
using SoundInTheory.Piranha.Identity.Extensions;
using SoundInTheory.Piranha.Identity.Extensions.Model;
using SoundInTheory.Piranha.Identity.Extensions.Services;
using SoundInTheory.Piranha.Identity.Extensions.Themes;

public static class IdentityExtensionsExtensions
{
    /// <summary>
    /// Adds the IdentityExtensions module.
    /// </summary>
    /// <param name="serviceBuilder"></param>
    /// <returns></returns>
    public static PiranhaServiceBuilder UseIdentityExtensions(this PiranhaServiceBuilder serviceBuilder)
    {
        serviceBuilder.Services.AddIdentityExtensions();

        return serviceBuilder;
    }

    /// <summary>
    /// Uses the IdentityExtensions module.
    /// </summary>
    /// <param name="applicationBuilder">The current application builder</param>
    /// <returns>The builder</returns>
    public static PiranhaApplicationBuilder UseIdentityExtensions(this PiranhaApplicationBuilder applicationBuilder)
    {

        applicationBuilder.Builder.UseIdentityExtensions();

        return applicationBuilder;
    }

    /// <summary>
    /// Adds the IdentityExtensions module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddIdentityExtensions(this IServiceCollection services)
    {
        // Add the IdentityExtensions module
        Piranha.App.Modules.Register<IdentityExtensionsModule>();

        services.AddRazorPages(opts =>
        {
            opts.Conventions.Add(new IdentityRoutingConvention());
        });


        services.AddScoped<LoginThemeService>();
        services.AddScoped<ILoginTheme, PiranhaTheme>();

        // Setup authorization policies
       

        // Return the service collection
        return services;
    }

    /// <summary>
    /// Uses the IdentityExtensions.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UseIdentityExtensions(this IApplicationBuilder builder)
    {
        return builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(typeof(IdentityExtensionsModule).Assembly, "IdentityExtensions.assets.dist"),
            RequestPath = "/manager/IdentityExtensions"
        });
    }

    /// <summary>
    /// Static accessor to IdentityExtensions module if it is registered in the Piranha application.
    /// </summary>
    /// <param name="modules">The available modules</param>
    /// <returns>The IdentityExtensions module</returns>
    public static IdentityExtensionsModule IdentityExtensions(this Piranha.Runtime.AppModuleList modules)
    {
        return modules.Get<IdentityExtensionsModule>();
    }
}
