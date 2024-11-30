using Template.Application.Common.Interfaces.Services;
using Template.Infra.Settings.Configurations;

namespace Template.Infra.ExternalServices.Google;

internal static class DependencyInjection
{
    public static IServiceCollection AddGoogleAPI(this IServiceCollection services, IConfiguration config)
    {
        var googleConfig = new GoogleConfiguration();
        config.GetSection($"{GoogleConfiguration.Key}:{GoogleConfiguration.GoogleKey}")
              .Bind(googleConfig);

        services.AddOptions<GoogleConfiguration>()
            .BindConfiguration($"{GoogleConfiguration.Key}:{GoogleConfiguration.GoogleKey}");

        services.AddScoped<IGoogle, Google>(x =>
                new Google(googleConfig)
        );

        return services;
    }
}