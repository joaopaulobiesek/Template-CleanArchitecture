using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Template.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> where TResponse : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUser _user;
    private readonly IIdentityService _identityService;

    public PerformanceBehaviour(ILogger<TRequest> logger, ICurrentUser user, IIdentityService identityService)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task<ApiResponse<TResponse>> Handle(Func<Task<ApiResponse<TResponse>>> executeCore, TRequest request, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await executeCore();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.Id ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId, userName, request);
        }

        return response;
    }
}