using FluentValidation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Template.Application.Common.Interfaces.Security;

namespace Template.Application.Common.Behaviours;

public class HandlerDependencies<TRequest, TResponse> where TResponse : notnull
{
    public ILogger<TRequest> Logger { get; }
    public ICurrentUser User { get; }
    public IIdentityService IdentityService { get; }
    public IEnumerable<IValidator<TRequest>> Validators { get; }
    public IHostEnvironment Environment { get; }
    public IServiceProvider ServiceProvider { get; }

    public HandlerDependencies(
        ILogger<TRequest> logger,
        ICurrentUser user,
        IIdentityService identityService,
        IEnumerable<IValidator<TRequest>> validators,
        IHostEnvironment environment,
        IServiceProvider serviceProvider)
    {
        Logger = logger;
        User = user;
        IdentityService = identityService;
        Validators = validators;
        Environment = environment;
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
}