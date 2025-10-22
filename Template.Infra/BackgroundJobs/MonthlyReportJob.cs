using Microsoft.Extensions.Logging;
using Template.Application.Common.Behaviours;
using Template.Application.Domains.V1.Alerts.Commands.SendMonthlyReport;

namespace Template.Infra.BackgroundJobs;

/// <summary>
/// Job Hangfire para enviar relatórios mensais
/// Executa dia 1 de cada mês às 9h (configurável)
/// </summary>
public class MonthlyReportJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MonthlyReportJob> _logger;

    public MonthlyReportJob(
        IServiceProvider serviceProvider,
        ILogger<MonthlyReportJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Executa envio de relatórios mensais
    /// </summary>
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("[MonthlyReportJob] Iniciando envio de relatórios mensais");

        try
        {
            using var scope = _serviceProvider.CreateScope();

            var handler = scope.ServiceProvider
                .GetRequiredService<IHandlerBase<SendMonthlyReportCommand, bool>>();

            var command = new SendMonthlyReportCommand();

            var result = await handler.Execute(command, CancellationToken.None);

            if (result.Success)
            {
                _logger.LogInformation("[MonthlyReportJob] Relatórios mensais enviados com sucesso: {Message}",
                    result.Message);
            }
            else
            {
                _logger.LogWarning("[MonthlyReportJob] Envio de relatórios mensais falhou: {Message}",
                    result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MonthlyReportJob] Erro ao executar envio de relatórios mensais");
            throw;
        }
    }
}
