using Template.Infra.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Template.Infra.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString(nameof(Context));

        services.AddDbContext<Context>(options
            => options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsHistoryTable(
                    tableName: HistoryRepository.DefaultTableName
                );
                sqlServerOptions.CommandTimeout(360);
            })
        );

        services.AddScoped<IContext>(sp => sp.GetRequiredService<Context>());
        services.AddScoped<IDapperConnection, Contexts.Dapper>();
        services.AddScoped<DatabaseInitializer>();

        return services;
    }
}