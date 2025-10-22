namespace Template.Infra.Settings.Configurations;

/// <summary>
/// Configurações para Background Jobs com Hangfire
/// </summary>
public class HangfireConfiguration
{
    /// <summary>
    /// Cron para verificar alertas (padrão: a cada hora)
    /// Exemplo: "0 * * * *" = todo início de hora
    /// </summary>
    public string AlertCheckCron { get; set; } = "0 * * * *";

    /// <summary>
    /// Cron para relatório semanal (padrão: domingos às 9h)
    /// Exemplo: "0 9 * * 0" = domingo às 9:00
    /// </summary>
    public string WeeklyReportCron { get; set; } = "0 9 * * 0";

    /// <summary>
    /// Cron para relatório mensal (padrão: dia 1 de cada mês às 9h)
    /// Exemplo: "0 9 1 * *" = dia 1 às 9:00
    /// </summary>
    public string MonthlyReportCron { get; set; } = "0 9 1 * *";

    /// <summary>
    /// Percentual para alerta de aviso (padrão: 80%)
    /// </summary>
    public decimal AlertThreshold80 { get; set; } = 80;

    /// <summary>
    /// Percentual para alerta crítico (padrão: 100%)
    /// </summary>
    public decimal AlertThreshold100 { get; set; } = 100;

    /// <summary>
    /// Habilitar logs detalhados de jobs
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;
}
