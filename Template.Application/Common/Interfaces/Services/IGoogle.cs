using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;

namespace Template.Application.Common.Interfaces.Services;

public interface IGoogle
{
    Task<ApiResponse<string>> AuthenticateUserAsync(IIdentityService identity, string code, Guid xTenantID);
    ApiResponse<string> GenerateAuthenticationUrl(string? state = null);
    Task<ApiResponse<List<GoogleCalendarEvent>>> GetGoogleCalendarEventsAsync(IIdentityService identity, string userId);
}