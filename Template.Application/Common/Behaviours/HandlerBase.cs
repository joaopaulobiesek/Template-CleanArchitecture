using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Common.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Template.Application.Common.Behaviours;

public interface IHandlerBase<TRequest, TResponse> where TResponse : notnull
{
    Task<ApiResponse<TResponse>> Execute(TRequest request, CancellationToken cancellationToken, object? additionalData = null);
}

public abstract class HandlerBase<TRequest, TResponse> : IHandlerBase<TRequest, TResponse>
    where TResponse : notnull
{
    protected IContext _context;
    protected ICurrentUser _user;
    private readonly ILogger<TRequest> _logger;
    private readonly IHostEnvironment _environment;
    private readonly IIdentityService _identityService;
    private readonly PerformanceBehaviour<TRequest, TResponse> _performanceBehaviour;
    private readonly UnhandledExceptionBehaviour<TRequest, TResponse> _exceptionBehaviour;
    private readonly ValidationBehaviour<TRequest, TResponse> _validationBehaviour;
    private readonly AuthorizationBehaviour<TRequest, TResponse> _authorizationBehaviour;
    private readonly LoggingBehaviour<TRequest> _loggingBehaviour;

    protected HandlerBase(IContext context, HandlerDependencies<TRequest, TResponse> dependencies)
    {
        _context = context;
        _logger = dependencies.Logger;
        _user = dependencies.User;
        _identityService = dependencies.IdentityService;
        _environment = dependencies.Environment;

        _authorizationBehaviour = new AuthorizationBehaviour<TRequest, TResponse>(_user, _identityService, _environment);
        _performanceBehaviour = new PerformanceBehaviour<TRequest, TResponse>(_logger, _user, _identityService);
        _exceptionBehaviour = new UnhandledExceptionBehaviour<TRequest, TResponse>(_logger);
        _validationBehaviour = new ValidationBehaviour<TRequest, TResponse>(dependencies.Validators);
        _loggingBehaviour = new LoggingBehaviour<TRequest>(_logger, _user, _identityService);
    }

    protected abstract Task<ApiResponse<TResponse>> RunCore(TRequest request, CancellationToken cancellationToken, object? additionalData);

    public async Task<ApiResponse<TResponse>> Execute(TRequest request, CancellationToken cancellationToken, object? additionalData = null)
    {
        await _loggingBehaviour.Process(request, cancellationToken);

        return await _performanceBehaviour.Handle(
            () => _exceptionBehaviour.Handle(
                () => _validationBehaviour.Handle(
                    () => _authorizationBehaviour.Handle(
                        () => RunCore(request, cancellationToken, additionalData),
                        request, cancellationToken),
                    request, cancellationToken),
                request, cancellationToken),
            request, cancellationToken);
    }
}