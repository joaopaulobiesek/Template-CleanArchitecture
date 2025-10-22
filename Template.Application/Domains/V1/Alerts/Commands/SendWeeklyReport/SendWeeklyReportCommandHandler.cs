using Microsoft.Extensions.Logging;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;

namespace Template.Application.Domains.V1.Alerts.Commands.SendWeeklyReport;

public class SendWeeklyReportCommandHandler : HandlerBase<SendWeeklyReportCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<SendWeeklyReportCommandHandler> _logger;

    public SendWeeklyReportCommandHandler(
        HandlerDependencies<SendWeeklyReportCommand, bool> dependencies,
        ILogger<SendWeeklyReportCommandHandler> logger) : base(dependencies)
    {
        _identityService = dependencies.IdentityService;
        _logger = logger;
    }

    protected override async Task<ApiResponse<bool>> RunCore(
        SendWeeklyReportCommand request,
        CancellationToken cancellationToken,
        object? additionalData = null)
    {

        return new SuccessResponse<bool>(
            $"Relatórios semanais processados. Enviados: , Falhas: ",
            true);
    }
}
