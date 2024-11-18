using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Common.Persistence;
using Template.Application.ViewModels.Users;

namespace Template.Application.Domains.Identity.Auth.Commands.LoginUser;

public class LoginUserCommandHandler : HandlerBase<LoginUserCommand, LoginUserVm>
{
    private readonly IIdentityService _service;

    public LoginUserCommandHandler(IContext context, HandlerDependencies<LoginUserCommand, LoginUserVm> dependencies) : base(context, dependencies)
    {
        _service = dependencies.IdentityService;
    }

    protected override async Task<ApiResponse<LoginUserVm>> RunCore(LoginUserCommand request, CancellationToken cancellationToken, object? additionalData = null)
    {
        var user = await _service.LoginAsync(request.Email, request.Password);
        var userVm = new LoginUserVm
        {
            Name = user.Item1,
            Email = user.Item2,
            Token = user.Item3
        };

        return new SuccessResponse<LoginUserVm>("Login realizado com sucesso!", userVm);
    }
}