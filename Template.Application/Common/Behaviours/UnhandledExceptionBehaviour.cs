using Template.Application.Common.Exceptions;
using Template.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace Template.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> where TResponse : notnull
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<ApiResponse<TResponse>> Handle(Func<Task<ApiResponse<TResponse>>> executeCore, TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await executeCore();
        }
        catch (UnauthorizedAccessException ex)
        {
            return new ErrorResponse<TResponse>(ex.Message, 401);
        }
        catch (ForbiddenAccessException ex)
        {
            return new ErrorResponse<TResponse>(ex.Message, 403);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(ex, "Unhandled Exception for Request {Name} {@Request}", requestName, request);

            return new ErrorResponse<TResponse>(ex.Message, 400);
        }
    }
}