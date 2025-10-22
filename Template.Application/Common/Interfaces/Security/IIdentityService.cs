using Template.Application.Common.Models;
using Template.Application.Domains.V1.ViewModels.Users;

namespace Template.Application.Common.Interfaces.Security;

public interface IIdentityService
{
    Task<(string, string, string, string)> LoginAsync(string emailUserName, string password);
    Task<string?> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<UserVm> CreateUserAsync(IUser user, string? password);
    Task<UserVm> EditUserAsync(IUser user, string? password);
    Task<ApiResponse<string>> DeleteUserAsync(string userId);
    IQueryable<UserVm>? ListUsersAsync(int order, string param, string? searchText = null, Dictionary<string, string>? customFilter = null);
    Task<List<string>> GetUserRole(string userId);
    Task<List<string>> GetUserPolicies(string userId);
    Task<ApiResponse<LoginGoogleResponse>> HandleExternalLoginAsync(string provider, string providerKey, string email, string name, string? phoneNumber, string? picture, string? state);
    Task<ApiResponse<string>> AddLoginProviderTokenAsync(string providerKey, string loginProvider, string tokenName, string tokenValue);
    Task<ApiResponse<string>> RemoveLoginProviderTokenAsync(string providerKey, string loginProvider, string tokenName);
    Task<ApiResponse<string>> GetLoginProviderTokenAsync(string userId, string loginProvider, string tokenName);
    Task<UserVm> SearchUserByEmailOrUserNameAsync(string emailUserName);
}