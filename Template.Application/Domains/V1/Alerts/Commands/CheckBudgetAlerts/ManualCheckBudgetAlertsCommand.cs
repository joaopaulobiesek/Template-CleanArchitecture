using Template.Application.Common.Security;
using Template.Domain.Constants;

namespace Template.Application.Domains.V1.Alerts.Commands.CheckBudgetAlerts;

/// <summary>
/// Command para ADMIN executar MANUALMENTE o job de verificação de alertas
/// Diferente do CheckBudgetAlertsCommand (executado pelo Hangfire sem autenticação),
/// este command requer autenticação de Admin
/// </summary>
[Authorize(Roles = Roles.Admin)]
public class ManualCheckBudgetAlertsCommand
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
