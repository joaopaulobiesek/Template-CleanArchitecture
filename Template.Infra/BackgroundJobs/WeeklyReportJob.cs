using Microsoft.Extensions.Logging;
using Template.Application.Common.Behaviours;
using Template.Application.Domains.V1.Alerts.Commands.SendWeeklyReport;

namespace Template.Infra.BackgroundJobs;

/// <summary>
/// Job Hangfire para enviar relatórios semanais
/// Executa todo domingo às 9h (configurável)
/// </summary>
public class WeeklyReportJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WeeklyReportJob> _logger;

    public WeeklyReportJob(
        IServiceProvider serviceProvider,
        ILogger<WeeklyReportJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Executa envio de relatórios semanais
    /// </summary>
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("[WeeklyReportJob] Iniciando envio de relatórios semanais");

        try
        {
            using var scope = _serviceProvider.CreateScope();

            var handler = scope.ServiceProvider
                .GetRequiredService<IHandlerBase<SendWeeklyReportCommand, bool>>();

            var command = new SendWeeklyReportCommand();

            var result = await handler.Execute(command, CancellationToken.None);

            if (result.Success)
            {
                _logger.LogInformation("[WeeklyReportJob] Relatórios semanais enviados com sucesso: {Message}",
                    result.Message);
            }
            else
            {
                _logger.LogWarning("[WeeklyReportJob] Envio de relatórios semanais falhou: {Message}",
                    result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[WeeklyReportJob] Erro ao executar envio de relatórios semanais");
            throw;
        }
    }
}
