using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Domains.V1.ViewModels.Users;

namespace Template.Application.Common.Interfaces.Services;

public interface IGoogle
{
    Task<ApiResponse<LoginUserVm>> AuthenticateUserAsync(IIdentityService identity, string code, string? state);
    ApiResponse<string> GenerateAuthenticationUrl(string? state = null);
    Task<ApiResponse<List<GoogleCalendarEvent>>> GetGoogleCalendarEventsAsync(IIdentityService identity, string userId);
}