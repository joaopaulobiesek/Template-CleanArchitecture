using Microsoft.Extensions.Logging;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;

namespace Template.Application.Domains.V1.Alerts.Commands.CheckBudgetAlerts;

public class CheckBudgetAlertsCommandHandler : HandlerBase<CheckBudgetAlertsCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<CheckBudgetAlertsCommandHandler> _logger;

    public CheckBudgetAlertsCommandHandler(
        HandlerDependencies<CheckBudgetAlertsCommand, bool> dependencies,
        ILogger<CheckBudgetAlertsCommandHandler> logger) : base(dependencies)
    {
        _identityService = dependencies.IdentityService;
        _logger = logger;
    }

    protected override async Task<ApiResponse<bool>> RunCore(
        CheckBudgetAlertsCommand request,
        CancellationToken cancellationToken,
        object? additionalData = null)
    {
        return new SuccessResponse<bool>(
            $"Alertas processados com sucesso. Warning: , Critical: ",
            true);
    }
}
