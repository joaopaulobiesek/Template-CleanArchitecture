using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;
using Template.Application.Common.Security;
using Template.Domain.Constants;

namespace Template.Application.Domains.V1.ExternalServices.Google.Queries.CalendarEvents;

[Authorize(Roles = Roles.User)]
[Authorize(Policy = Policies.CanView)]
public class GetCalendarEventsQuery
{
}

public class GetCalendarEventsQueryHandler : HandlerBase<GetCalendarEventsQuery, List<GoogleCalendarEvent>>
{
    private readonly IGoogle _google;
    private readonly IIdentityService _service;

    public GetCalendarEventsQueryHandler(HandlerDependencies<GetCalendarEventsQuery, List<GoogleCalendarEvent>> dependencies, IGoogle google) : base(dependencies)
    {
        _google = google;
        _service = dependencies.IdentityService;
    }

    protected override async Task<ApiResponse<List<GoogleCalendarEvent>>> RunCore(GetCalendarEventsQuery request, CancellationToken cancellationToken, object? additionalData = null)
        => await _google.GetGoogleCalendarEventsAsync(_service, _user.Id!);
}