namespace Template.Application.Domains.V1.Alerts.Commands.CheckBudgetAlerts;

/// <summary>
/// Command para verificar e disparar alertas de orçamento (80% e 100%)
/// Executado periodicamente pelo Hangfire job
/// Processo:
/// 1. Busca todos os EnvelopeBudgets ativos do mês atual
/// 2. Calcula % usado: (SpentAmount / AllocatedAmount) * 100
/// 3. Se >= 80% e < 100% → cria alerta Warning80 + envia WhatsApp
/// 4. Se >= 100% → cria alerta Critical100 + envia WhatsApp
/// 5. Não envia alertas duplicados (verifica se já existe para o envelope)
/// </summary>
public class CheckBudgetAlertsCommand
{
    /// <summary>
    /// Limiar para alerta de aviso (padrão: 80%)
    /// </summary>
    public decimal WarningThreshold { get; set; } = 80;

    /// <summary>
    /// Limiar para alerta crítico (padrão: 100%)
    /// </summary>
    public decimal CriticalThreshold { get; set; } = 100;
}
