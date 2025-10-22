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

        if (string.IsNullOrEmpty(user.Item4))
            return new ErrorResponse<LoginUserVm>("Senha ou email inválido!");

        var roles = await _service.GetUserRole(user.Item4);
        var policies = await _service.GetUserPolicies(user.Item4);

        // Módulos removidos - Client/ClientModule desabilitados
        var modules = new List<string>();

        var userVm = new LoginUserVm
        {
            Name = user.Item1,
            Email = user.Item2,
            Modules = modules,
            Roles = roles,
            Policies = policies,
            Token = user.Item3
        };

        return new SuccessResponse<LoginUserVm>("Login realizado com sucesso!", userVm);
    }
}