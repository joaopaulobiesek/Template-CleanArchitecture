using Microsoft.EntityFrameworkCore.Migrations;
using Template.Infra.Persistence.Contexts.Tenant;
using System.Runtime.InteropServices;

namespace Template.Infra.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration config)
    {
        bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        string connectionString_TempMigrations = isMac ? "TempMigrations_MAC" : "TempMigrations";
        string connectionString_TenantContext = isMac ? "TenantContext_MAC" : "TenantContext";

        var connectionString = config.GetConnectionString(connectionString_TenantContext);

        services.AddDbContext<TenantContext>(options
            => options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsHistoryTable(
                    tableName: HistoryRepository.DefaultTableName
                );
                sqlServerOptions.CommandTimeout(360);
            })
        );

        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());

        services.AddScoped<ITenantDapperConnection, TenantDapper>();

        services
            .AddDefaultIdentity<ContextUser>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredLength = 3;
                o.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<ContextRole>()
            .AddErrorDescriber<IdentityPortugueseMessages>()
            .AddEntityFrameworkStores<TenantContext>();

        return services;
    }
}