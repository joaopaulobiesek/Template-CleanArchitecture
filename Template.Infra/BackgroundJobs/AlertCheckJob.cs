using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Template.Application.Common.Behaviours;
using Template.Application.Domains.V1.Alerts.Commands.CheckBudgetAlerts;
using Template.Infra.Settings.Configurations;

namespace Template.Infra.BackgroundJobs;

/// <summary>
/// Job Hangfire para verificar alertas de orçamento (80% e 100%)
/// Executa periodicamente (padrão: a cada hora)
/// </summary>
public class AlertCheckJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AlertCheckJob> _logger;
    private readonly HangfireConfiguration _config;

    public AlertCheckJob(
        IServiceProvider serviceProvider,
        ILogger<AlertCheckJob> logger,
        IOptions<HangfireConfiguration> config)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = config.Value;
    }

    /// <summary>
    /// Executa verificação de alertas
    /// </summary>
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("[AlertCheckJob] Iniciando verificação de alertas de orçamento");

        try
        {
            using var scope = _serviceProvider.CreateScope();

            var handler = scope.ServiceProvider
                .GetRequiredService<IHandlerBase<CheckBudgetAlertsCommand, bool>>();

            var command = new CheckBudgetAlertsCommand
            {
                WarningThreshold = _config.AlertThreshold80,
                CriticalThreshold = _config.AlertThreshold100
            };

            var result = await handler.Execute(command, CancellationToken.None);

            if (result.Success)
            {
                _logger.LogInformation("[AlertCheckJob] Verificação de alertas concluída com sucesso: {Message}",
                    result.Message);
            }
            else
            {
                _logger.LogWarning("[AlertCheckJob] Verificação de alertas falhou: {Message}",
                    result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AlertCheckJob] Erro ao executar verificação de alertas");
            throw; // Hangfire vai registrar a falha
        }
    }
}
