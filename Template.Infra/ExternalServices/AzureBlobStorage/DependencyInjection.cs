using Azure.Storage.Blobs;
using Template.Application.Common.Interfaces.Services;
using Template.Infra.ExternalServices.AzureBlobStorage;
using Template.Infra.Settings.Configurations;

namespace Template.Infra.ExternalServices.Storage;

internal static class DependencyInjection
{
    public static IServiceCollection AdicionarStorage(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAzureStorage>(provider =>
        {
            var storageConfig = new StorageConfiguration();
            config.GetSection(StorageConfiguration.Key).Bind(storageConfig);

            var client = new BlobContainerClient(storageConfig.ConnectionString, storageConfig.ContainerName);
            var clientTemp = new BlobContainerClient(storageConfig.ConnectionString, storageConfig.TempContainerName);

            return new AzureStorage(client, clientTemp);
        });

        return services;
    }
}
