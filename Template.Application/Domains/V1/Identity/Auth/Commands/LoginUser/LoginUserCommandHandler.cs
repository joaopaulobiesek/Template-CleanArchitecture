using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.IRepositories.Tenant.Implementations;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Domains.V1.ViewModels.Users;

namespace Template.Application.Domains.V1.Identity.Auth.Commands.LoginUser;

public class LoginUserCommandHandler : HandlerBase<LoginUserCommand, LoginUserVm>
{
    private readonly IIdentityService _service;
    private readonly IClientRepository _repositoryClient;

    public LoginUserCommandHandler(HandlerDependencies<LoginUserCommand, LoginUserVm> dependencies, IClientRepository repositoryClient) : base(dependencies)
    {
        _service = dependencies.IdentityService;
        _repositoryClient = repositoryClient;
    }

    protected override async Task<ApiResponse<LoginUserVm>> RunCore(LoginUserCommand request, CancellationToken cancellationToken, object? additionalData = null)
    {
        var user = await _service.LoginAsync(request.Email, request.Password);

        var modules = new List<string>();
        if (!string.IsNullOrEmpty(this._user.Id))
        {
            modules = await _repositoryClient.GetActiveModulesAsync(this._user.Id);
        }

        var userVm = new LoginUserVm
        {
            Name = user.Item1,
            Email = user.Item2,
            Modules = modules,
            Token = user.Item3
        };

        return new SuccessResponse<LoginUserVm>("Login realizado com sucesso!", userVm);
    }
}