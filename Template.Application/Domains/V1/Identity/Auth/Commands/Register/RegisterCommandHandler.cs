using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Domains.V1.ViewModels.Users;
using Template.Domain.Constants;

namespace Template.Application.Domains.V1.Identity.Auth.Commands.Register;

public class RegisterCommandHandler : HandlerBase<RegisterCommand, LoginUserVm>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(HandlerDependencies<RegisterCommand, LoginUserVm> dependencies) : base(dependencies)
    {
        _identityService = dependencies.IdentityService;
    }

    protected override async Task<ApiResponse<LoginUserVm>> RunCore(
        RegisterCommand request,
        CancellationToken cancellationToken,
        object? additionalData = null)
    {
        var newUser = new UserVm
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Roles = new List<string> { Roles.User },
            Policies = new List<string> { Policies.CanList, Policies.CanView, Policies.CanCreate, Policies.CanEdit, Policies.CanDelete, Policies.CanViewReports }
        };

        var createdUser = await _identityService.CreateUserAsync(newUser, request.Password);

        if (string.IsNullOrEmpty(createdUser.Id))
        {
            return new ErrorResponse<LoginUserVm>("Failed to create user.", 400);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // 5. Fazer login automático e retornar JWT
        var loginResult = await _identityService.LoginAsync(request.Email, request.Password);

        var roles = await _identityService.GetUserRole(loginResult.Item4);
        var policies = await _identityService.GetUserPolicies(loginResult.Item4);

        var userVm = new LoginUserVm
        {
            Name = loginResult.Item1,
            Email = loginResult.Item2,
            Modules = new List<string>(),
            Roles = roles,
            Policies = policies,
            Token = loginResult.Item3
        };

        return new SuccessResponse<LoginUserVm>(
            "User registered successfully with invite code.",
            userVm);
    }
}
