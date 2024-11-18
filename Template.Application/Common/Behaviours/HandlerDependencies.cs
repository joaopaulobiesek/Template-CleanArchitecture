using Template.Application.Common.Interfaces.Security;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Template.Application.Common.Behaviours;

public class HandlerDependencies<TRequest, TResponse> where TResponse : notnull
{
    public ILogger<TRequest> Logger { get; }
    public ICurrentUser User { get; }
    public IIdentityService IdentityService { get; }
    public IEnumerable<IValidator<TRequest>> Validators { get; }
    public IHostEnvironment Environment { get; }

    public HandlerDependencies(
        ILogger<TRequest> logger,
        ICurrentUser user,
        IIdentityService identityService,
        IEnumerable<IValidator<TRequest>> validators,
        IHostEnvironment environment)
    {
        Logger = logger;
        User = user;
        IdentityService = identityService;
        Validators = validators;
        Environment = environment;
    }
}