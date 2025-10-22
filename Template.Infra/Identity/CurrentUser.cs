using Microsoft.AspNetCore.Http;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Security;

namespace Template.Infra.Identity;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id
    {
        get
        {
            var internalUserId = InternalAuthContext.GetUserId();
            if (!string.IsNullOrEmpty(internalUserId))
                return internalUserId;

            // Se não, busca do HttpContext (requisições HTTP normais)
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

    public string? GroupName
    {
        get
        {
            return _httpContextAccessor.HttpContext?.Items["GroupName"]?.ToString();
        }
    }
}