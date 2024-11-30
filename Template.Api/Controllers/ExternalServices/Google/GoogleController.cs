using Microsoft.AspNetCore.Mvc;
using Template.Api.Controllers.System;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Models;
using Template.Application.Domains.ExternalServices.Google.Queries.CalendarEvents;

namespace Template.Api.Controllers.ExternalServices.Google;

[ApiController, Route("[controller]")]
public class GoogleController : BaseController
{
    /// <summary>
    /// Obtém os eventos do Google Calendar do usuário logado.
    /// </summary>
    /// <param name="handler">Handler responsável por executar a lógica de busca dos eventos do Google Calendar.</param>
    /// <param name="cancellationToken">Token para cancelar a operação, caso necessário.</param>
    /// <returns>
    /// Retorna uma resposta contendo a lista de eventos do Google Calendar ou erros, caso ocorram.
    /// </returns>
    [HttpGet("Calendar/Events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<List<GoogleCalendarEvent>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<List<GoogleCalendarEvent>>))]
    public async Task<IActionResult> GetGoogleCalendarEventsAsync(
        [FromServices] IHandlerBase<GetCalendarEventsQuery, List<GoogleCalendarEvent>> handler,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(new GetCalendarEventsQuery(), cancellationToken));
}