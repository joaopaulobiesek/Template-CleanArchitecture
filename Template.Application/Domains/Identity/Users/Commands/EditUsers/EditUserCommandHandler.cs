using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Common.Persistence;

namespace Template.Application.Domains.Identity.Users.Commands.EditUsers;

public class EditUserCommandHandler : HandlerBase<EditUserCommand, User>
{
    private readonly IIdentityService _identity;

    public EditUserCommandHandler(IContext context, HandlerDependencies<EditUserCommand, User> dependencies) : base(context, dependencies)
    {
        _identity = dependencies.IdentityService;
    }

    protected override async Task<ApiResponse<User>> RunCore(EditUserCommand request, CancellationToken cancellationToken, object? additionalData)
    {
        var result = await _identity.EditUserAsync(
           new User()
           {
               FullName = request.FullName,
               Email = request.Email,
               Roles = request.Roles!,
               Policies = request.Policies!,
               ProfileImageUrl = request.ProfileImageUrl
           },
           request.Password
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

        return new ErrorResponse<User>("Ok");
    }
}