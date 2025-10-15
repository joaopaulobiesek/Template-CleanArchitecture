using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;

namespace Template.Application.Domains.V1.Identity.Users.Commands.CreateUsers;

public class CreateUserCommandHandler : HandlerBase<CreateUserCommand, User>
{
    private readonly IIdentityService _identity;

    public CreateUserCommandHandler(HandlerDependencies<CreateUserCommand, User> dependencies) : base(dependencies)
    {
        _identity = dependencies.IdentityService;
    }

    protected override async Task<ApiResponse<User>> RunCore(CreateUserCommand request, CancellationToken cancellationToken, object? additionalData)
    {
        var result = await _identity.CreateUserAsync(
            new User()
            {
                FullName = request.FullName,
                Email = request.Email,
                Roles = request.Roles!,
                Policies = request.Policies!,
                ProfileImageUrl = request.ProfileImageUrl
            },
            request.Password!
        );

        if (result != null)
            return new SuccessResponse<User>("Ok",
                new User
                {
                    Id = result.Id,
                    Email = result.Email,
                    FullName = result.FullName,
                    ProfileImageUrl = result.ProfileImageUrl,
                    Policies = result.Policies,
                    Roles = result.Roles
                }
            );

        return new ErrorResponse<User>("Deu erro");
    }
}