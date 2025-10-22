using Template.Application.Common.Security;
using Template.Domain.Constants;

namespace Template.Application.Domains.V1.Alerts.Commands.SendWeeklyReport;

/// <summary>
/// Command para ADMIN executar MANUALMENTE o job de envio de relatórios semanais
/// Diferente do SendWeeklyReportCommand (executado pelo Hangfire sem autenticação),
/// este command requer autenticação de Admin
/// </summary>
[Authorize(Roles = Roles.Admin)]
public class ManualSendWeeklyReportCommand
{
}
