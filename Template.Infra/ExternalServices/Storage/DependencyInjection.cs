using Azure.Storage.Blobs;
using Template.Application.Common.Interfaces.Services;
using Template.Infra.Settings.Configurations;

namespace Template.Infra.ExternalServices.Storage;

internal static class DependencyInjection
{
    public static IServiceCollection AdicionarStorage(this IServiceCollection services, IConfiguration config)
    {
        var storageConfigration = new StorageConfiguration();
        config.GetSection(StorageConfiguration.Key).Bind(storageConfigration);

        services.AddOptions<StorageConfiguration>()
            .BindConfiguration(StorageConfiguration.Key);

        services.AddScoped(x => new BlobContainerClient(storageConfigration.ConnectionString, storageConfigration.ContainerName));

        services.AddScoped<IStorage, AzureStorage>();
        return services;
    }
}
