using Template.Infra.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Runtime.InteropServices;

namespace Template.Infra.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration config)
    {
        bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        string connectionKey = isMac ? "Context_MAC" : "Context";

        var connectionString = config.GetConnectionString(connectionKey);

        bool isEfCommand = Environment.GetCommandLineArgs().Any(arg => arg.Contains("ef", StringComparison.OrdinalIgnoreCase));

        services.AddDbContext<Context>(options =>
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                sqlServerOptions.CommandTimeout(360);
            }));

        services.AddScoped<IContext>(sp => sp.GetRequiredService<Context>());
        services.AddScoped<IDapperConnection, Contexts.Dapper>();
        services.AddScoped<DatabaseInitializer>();

        return services;
    }
}
