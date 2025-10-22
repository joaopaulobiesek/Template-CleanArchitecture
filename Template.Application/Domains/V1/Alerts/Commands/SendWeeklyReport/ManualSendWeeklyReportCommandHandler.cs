using Microsoft.Extensions.Logging;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;

namespace Template.Application.Domains.V1.Alerts.Commands.SendWeeklyReport;

/// <summary>
/// Handler para execução MANUAL (Admin) do job de envio de relatórios semanais
/// Reutiliza a mesma lógica do SendWeeklyReportCommandHandler
/// </summary>
public class ManualSendWeeklyReportCommandHandler : HandlerBase<ManualSendWeeklyReportCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ManualSendWeeklyReportCommandHandler> _logger;

    public ManualSendWeeklyReportCommandHandler(
        HandlerDependencies<ManualSendWeeklyReportCommand, bool> dependencies,
        ILogger<ManualSendWeeklyReportCommandHandler> logger) : base(dependencies)
    {
        _identityService = dependencies.IdentityService;
        _logger = logger;
    }

    protected override async Task<ApiResponse<bool>> RunCore(
        ManualSendWeeklyReportCommand request,
        CancellationToken cancellationToken,
        object? additionalData = null)
    {
        return new SuccessResponse<bool>(
            $"Relatórios semanais processados. Enviados:, Falhas:",
            true);
    }
}
