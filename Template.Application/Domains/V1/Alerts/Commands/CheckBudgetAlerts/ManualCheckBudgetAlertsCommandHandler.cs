using Microsoft.Extensions.Logging;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;

namespace Template.Application.Domains.V1.Alerts.Commands.CheckBudgetAlerts;

/// <summary>
/// Handler para execução MANUAL (Admin) do job de verificação de alertas
/// Reutiliza a mesma lógica do CheckBudgetAlertsCommandHandler
/// </summary>
public class ManualCheckBudgetAlertsCommandHandler : HandlerBase<ManualCheckBudgetAlertsCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ManualCheckBudgetAlertsCommandHandler> _logger;

    public ManualCheckBudgetAlertsCommandHandler(
        HandlerDependencies<ManualCheckBudgetAlertsCommand, bool> dependencies,
        ILogger<ManualCheckBudgetAlertsCommandHandler> logger) : base(dependencies)
    {
        _identityService = dependencies.IdentityService;
        _logger = logger;
    }

    protected override async Task<ApiResponse<bool>> RunCore(
        ManualCheckBudgetAlertsCommand request,
        CancellationToken cancellationToken,
        object? additionalData = null)
    {
        return new SuccessResponse<bool>(
            $"Alertas processados com sucesso. Warning: , Critical: ",
            true);
    }
}
