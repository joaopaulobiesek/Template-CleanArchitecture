using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Domains.V1.ViewModels.Users;
using Template.Domain.Constants;
using System.Linq.Expressions;

namespace Template.Infra.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ContextUser> _userManager;
    private readonly ITokenService _tokenService;

    public IdentityService(
        UserManager<ContextUser> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<(string, string, string)> LoginAsync(string emailUserName, string password)
    {
        var user = await SearchUserAsync(emailUserName);

        if (user is null)
            return (string.Empty, string.Empty, string.Empty);

        var checkPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!checkPassword)
            return (string.Empty, string.Empty, string.Empty);

        return (user.FullName, user.Email, await GenerateTokensAndUpdateUserAsync(user))!;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<UserVm> CreateUserAsync(IUser user, string? password)
    {
        var newUser = new ContextUser
        {
            FullName = user.FullName,
            UserName = user.Email,
            Email = user.Email,
            ProfileImageUrl = user.ProfileImageUrl,
        };

        var result = new IdentityResult();

        if (string.IsNullOrEmpty(password))
            result = await _userManager.CreateAsync(newUser);
        else
            result = await _userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
            return new UserVm();

        newUser = await SearchUserAsync(user.Email!);

        if (newUser == null)
            return new UserVm();

        var createRoles = await _userManager.AddToRolesAsync(newUser, user.Roles!);

        if (!createRoles.Succeeded)
            return new UserVm();

        var claims = user.Policies?.Select(policy => new Claim("Permission", policy)).ToList();
        if (claims != null)
        {
            var addClaimsResult = await _userManager.AddClaimsAsync(newUser, claims);
            if (!addClaimsResult.Succeeded)
                return new UserVm();
        }

        return new UserVm(newUser.Id, newUser.Email);
    }

    public async Task<UserVm> EditUserAsync(IUser user, string? password)
    {
        var editUser = await _userManager.FindByIdAsync(user.Id!);

        if (editUser is null)
            return new UserVm();

        editUser.Update(user);

        var result = await _userManager.UpdateAsync(editUser);
        if (!result.Succeeded)
            return new UserVm();

        if (!string.IsNullOrWhiteSpace(password))
        {
            var removePasswordResult = await _userManager.RemovePasswordAsync(editUser);
            if (!removePasswordResult.Succeeded)
                return new UserVm();

            var addPasswordResult = await _userManager.AddPasswordAsync(editUser, password);
            if (!addPasswordResult.Succeeded)
                return new UserVm();
        }

        var currentRoles = await _userManager.GetRolesAsync(editUser);
        var rolesToAdd = user.Roles.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(user.Roles).ToList();

        if (rolesToAdd.Any())
        {
            var addRolesResult = await _userManager.AddToRolesAsync(editUser, rolesToAdd);
            if (!addRolesResult.Succeeded)
                return new UserVm();
        }

        if (rolesToRemove.Any())
        {
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(editUser, rolesToRemove);
            if (!removeRolesResult.Succeeded)
                return new UserVm();
        }

        var currentClaims = await _userManager.GetClaimsAsync(editUser);
        var currentPolicies = currentClaims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();

        var policiesToAdd = user.Policies.Except(currentPolicies)
            .Select(policy => new Claim("Permission", policy))
            .ToList();

        var policiesToRemove = currentClaims
            .Where(c => c.Type == "Permission" && !user.Policies.Contains(c.Value))
            .ToList();

        if (policiesToAdd.Any())
        {
            var addClaimsResult = await _userManager.AddClaimsAsync(editUser, policiesToAdd);
            if (!addClaimsResult.Succeeded)
                return new UserVm();
        }

        if (policiesToRemove.Any())
        {
            var removeClaimsResult = await _userManager.RemoveClaimsAsync(editUser, policiesToRemove);
            if (!removeClaimsResult.Succeeded)
                return new UserVm();
        }

        return new UserVm(editUser.Id, editUser.Email);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var userClaims = await _userManager.GetClaimsAsync(user);

        var hasPolicy = userClaims.Any(claim => claim.Type == "Permission" && claim.Value == policyName);

        return hasPolicy;
    }

    public async Task<ApiResponse<string>> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var result = await DeleteUserAsync(user);

            if (result.Success)
                return new SuccessResponse<string>("User deleted successfully");
            else
                return new ErrorResponse<string>("Failed to delete user", 400, null, (result as ErrorResponse<IdentityResult>)?.Errors);
        }
        else
        {
            return new ErrorResponse<string>("User not found", 404);
        }
    }

    public async Task<ApiResponse<IdentityResult>> DeleteUserAsync(ContextUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public IQueryable<UserVm>? ListUsersAsync(int order, string param, string? searchText = null)
    {
        IQueryable<ContextUser> userQuery = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            userQuery = userQuery.Where(x =>
            (x.FullName != null && x.FullName.ToUpper().Contains(searchText.ToUpper())) ||
            (x.Email != null && x.Email.ToUpper().Contains(searchText.ToUpper()))
            );
        }

        if (order == -1)
            userQuery = userQuery.OrderByDescending(SearchOrderProperty(param!));
        else
            userQuery = userQuery.OrderBy(SearchOrderProperty(param!));

        return userQuery.Select(u => new UserVm(u.Id, u.Email, u.FullName, u.ProfileImageUrl, null, null));
    }

    public async Task<List<string>> GetUserRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null) return new List<string>();

        var roles = await _userManager.GetRolesAsync(user);

        if (roles is null) return new List<string>();

        return roles.ToList();
    }

    public async Task<List<string>> GetUserPolicies(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null) return new List<string>();

        var claims = await _userManager.GetClaimsAsync(user);

        // Filtra apenas as claims de permission (policies) e retorna como uma lista de strings
        return claims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();
    }

    public async Task<ApiResponse<string>> HandleExternalLoginAsync(string provider, string providerKey, string email, string name, string? picture, Guid xTenantID)
    {
        var user = await _userManager.FindByLoginAsync(provider, providerKey);
        if (user != null)
            return new SuccessResponse<string>("Login with external provider succeeded.", await GenerateTokensAndUpdateUserAsync(user!));

        user = await SearchUserAsync(email);

        if (user == null)
        {
            var roles = new List<string> { Roles.User };
            var policies = new List<string> { Policies.CanList, Policies.CanView };

            user = new ContextUser
            {
                UserName = email,
                Email = email,
                FullName = name,
                ProfileImageUrl = picture,
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return new ErrorResponse<string>(
                    "Failed to create user.",
                    400,
                    null,
                    createResult.Errors.Select(e => new NotificationError("UserCreation", e.Description)).ToList()
                );

            user = await SearchUserAsync(email);

            var createRolesResult = await _userManager.AddToRolesAsync(user!, roles);
            if (!createRolesResult.Succeeded)
                return new ErrorResponse<string>(
                    "Failed to assign roles to user.",
                    400,
                    null,
                    createRolesResult.Errors.Select(e => new NotificationError("RoleAssignment", e.Description)).ToList()
                );

            var claims = policies.Select(policy => new Claim("Permission", policy)).ToList();
            if (claims.Any())
            {
                var addClaimsResult = await _userManager.AddClaimsAsync(user!, claims);
                if (!addClaimsResult.Succeeded)
                    return new ErrorResponse<string>(
                        "Failed to add claims to user.",
                        400,
                        null,
                        addClaimsResult.Errors.Select(e => new NotificationError("ClaimsAssignment", e.Description)).ToList()
                    );
            }
        }

        var addLoginResult = await _userManager.AddLoginAsync(user!, new UserLoginInfo(provider, providerKey, provider));
        if (!addLoginResult.Succeeded)
        {
            return new ErrorResponse<string>(
                "Failed to associate external login with user.",
                400,
                null,
                addLoginResult.Errors.Select(e => new NotificationError("ExternalLogin", e.Description)).ToList()
            );
        }

        return new SuccessResponse<string>("External login successfully associated with user.", await GenerateTokensAndUpdateUserAsync(user!));
    }

    public async Task<ApiResponse<string>> AddLoginProviderTokenAsync(string providerKey, string loginProvider, string tokenName, string tokenValue)
    {
        var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
        if (user == null)
            return new ErrorResponse<string>($"User with ID '{providerKey}' not found.");

        var result = await _userManager.SetAuthenticationTokenAsync(user, loginProvider, tokenName, tokenValue);
        if (!result.Succeeded)
            return new ErrorResponse<string>(
                "Failed to save authentication token.",
                400,
                null,
                result.Errors.Select(e => new NotificationError("ExternalLogin", e.Description)).ToList()
            );

        return new SuccessResponse<string>("External Login Ok!");
    }

    public async Task<ApiResponse<string>> RemoveLoginProviderTokenAsync(string providerKey, string loginProvider, string tokenName)
    {
        var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
        if (user == null)
            return new ErrorResponse<string>($"User with ID '{providerKey}' not found.");

        var result = await _userManager.RemoveAuthenticationTokenAsync(user, loginProvider, tokenName);
        if (!result.Succeeded)
            return new ErrorResponse<string>("Failed to remove authentication token.");

        return new SuccessResponse<string>("Success to remove authentication token.");
    }

    public async Task<ApiResponse<string>> GetLoginProviderTokenAsync(string userId, string loginProvider, string tokenName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new ErrorResponse<string>($"User with ID '{userId}' not found.");

        return new SuccessResponse<string>(
            "Success to get authentication token.",
            await _userManager.GetAuthenticationTokenAsync(user, loginProvider, tokenName)
        );
    }

    public async Task<UserVm> SearchUserByEmailOrUserNameAsync(string emailUserName)
    {
        var user = await SearchUserAsync(emailUserName);

        if (user == null)
            return new UserVm();

        return new UserVm(user.Id, user.Email);
    }

    private async Task<ContextUser?> SearchUserAsync(string emailUserName) =>
        emailUserName.Contains('@')
            ? await _userManager.FindByEmailAsync(emailUserName)
            : await _userManager.FindByNameAsync(emailUserName);

    private async Task<string> GenerateTokensAndUpdateUserAsync(ContextUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwt(user, roles.Select(r => r.ToUpper()).ToList());
        await _userManager.UpdateAsync(user);
        return token;
    }

    private static Expression<Func<ContextUser, object>> SearchOrderProperty(string param)
        => param?.ToLower() switch
        {
            "email" => user => user.Email!,
            _ => user => user.FullName
        };
}