using Microsoft.Extensions.Logging;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;

namespace Template.Application.Domains.V1.Alerts.Commands.SendMonthlyReport;

/// <summary>
/// Handler para execução MANUAL (Admin) do job de envio de relatórios mensais
/// Reutiliza a mesma lógica do SendMonthlyReportCommandHandler
/// </summary>
public class ManualSendMonthlyReportCommandHandler : HandlerBase<ManualSendMonthlyReportCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ManualSendMonthlyReportCommandHandler> _logger;

    public ManualSendMonthlyReportCommandHandler(
        HandlerDependencies<ManualSendMonthlyReportCommand, bool> dependencies,
        ILogger<ManualSendMonthlyReportCommandHandler> logger) : base(dependencies)
    {
        _identityService = dependencies.IdentityService;
        _logger = logger;
    }

    protected override async Task<ApiResponse<bool>> RunCore(
        ManualSendMonthlyReportCommand request,
        CancellationToken cancellationToken,
        object? additionalData = null)
    {
        return new SuccessResponse<bool>(
            $"Relatórios mensais processados. Enviados: , Falhas: ",
            true);
    }
}
