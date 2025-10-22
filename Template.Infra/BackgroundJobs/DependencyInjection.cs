using Hangfire;
using Hangfire.SqlServer;
using Template.Infra.Settings.Configurations;

namespace Template.Infra.BackgroundJobs;

public static class DependencyInjection
{
    /// <summary>
    /// Configura Hangfire e registra Background Jobs
    /// </summary>
    public static IServiceCollection AddHangfireJobs(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind configurações
        services.AddOptions<HangfireConfiguration>()
            .BindConfiguration("BackgroundJobs");

        // Obtem connection string
        var connectionString = configuration.GetConnectionString("TenantContext");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'TenantContext' não encontrada");
        }

        // Configura Hangfire com SQL Server
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
                SchemaName = "Hangfire"
            }));

        // Adiciona servidor Hangfire
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5; // Número de workers simultâneos
            options.ServerName = $"ChatbotWorker-{Environment.MachineName}";
        });

        // Registra Jobs como Transient
        services.AddTransient<AlertCheckJob>();
        services.AddTransient<WeeklyReportJob>();
        services.AddTransient<MonthlyReportJob>();

        return services;
    }

    /// <summary>
    /// Configura jobs recorrentes do Hangfire
    /// Chamado após o app.Build() no Program.cs
    /// </summary>
    public static IServiceProvider ConfigureRecurringJobs(this IServiceProvider serviceProvider)
    //public static void ConfigureRecurringJobs(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var hangfireConfig = configuration.GetSection("BackgroundJobs").Get<HangfireConfiguration>()
                             ?? new HangfireConfiguration();

        // Job 1: Verificar alertas de orçamento (a cada hora)
        RecurringJob.AddOrUpdate<AlertCheckJob>(
            "check-budget-alerts",
            job => job.ExecuteAsync(),
            hangfireConfig.AlertCheckCron,
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            });

        // Job 2: Relatório semanal (domingos às 9h)
        RecurringJob.AddOrUpdate<WeeklyReportJob>(
            "send-weekly-reports",
            job => job.ExecuteAsync(),
            hangfireConfig.WeeklyReportCron,
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            });

        // Job 3: Relatório mensal (dia 1 às 9h)
        RecurringJob.AddOrUpdate<MonthlyReportJob>(
            "send-monthly-reports",
            job => job.ExecuteAsync(),
            hangfireConfig.MonthlyReportCron,
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            });

        return serviceProvider;
    }
}
