using Template.Application.Common.Interfaces.Services;
using Template.Infra.Settings.Configurations;

namespace Template.Infra.ExternalServices.SendEmails;

internal static class DependencyInjection
{
    public static IServiceCollection AdicionarSendGrid(this IServiceCollection services, IConfiguration config)
    {
        var sendGridConfigration = new SendGridConfiguration();
        config.GetSection(SendGridConfiguration.Key).Bind(sendGridConfigration);

        services.AddOptions<SendGridConfiguration>()
            .BindConfiguration(SendGridConfiguration.Key);

        services.AddScoped<ISendGrid, SendGrid>(x => new SendGrid(sendGridConfigration.API_KEY!, sendGridConfigration.EmailDocumento!));
        return services;
    }
}