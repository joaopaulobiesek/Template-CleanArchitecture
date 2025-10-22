using Microsoft.Extensions.Logging;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;

namespace Template.Application.Domains.V1.Alerts.Commands.SendMonthlyReport;

public class SendMonthlyReportCommandHandler : HandlerBase<SendMonthlyReportCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<SendMonthlyReportCommandHandler> _logger;

    public SendMonthlyReportCommandHandler(
        HandlerDependencies<SendMonthlyReportCommand, bool> dependencies,
        ILogger<SendMonthlyReportCommandHandler> logger) : base(dependencies)
    {
        _identityService = dependencies.IdentityService;
        _logger = logger;
    }

    protected override async Task<ApiResponse<bool>> RunCore(
        SendMonthlyReportCommand request,
        CancellationToken cancellationToken,
        object? additionalData = null)
    {
        return new SuccessResponse<bool>(
            $"Relatórios mensais processados. Enviados: , Falhas:",
            true);
    }
}
