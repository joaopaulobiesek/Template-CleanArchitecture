using Microsoft.AspNetCore.Mvc;
using Template.Api.Controllers.System;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Models;
using Template.Application.Domains.V1.ExternalServices.Google.Queries.CalendarEvents;

namespace Template.Api.Controllers.V1.ExternalServices.Google;

[ApiController]
[Route("api/v1/[controller]")]
public class GoogleController : BaseController
{
    /// <summary>
    /// Obtém os eventos do Google Calendar do usuário logado.
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna a lista de eventos do Google Calendar associados à conta do usuário autenticado.
    /// 
    /// **Regras de negócio:**
    /// - O usuário deve estar autenticado e possuir permissões para acessar os eventos do Google Calendar.
    /// - O sistema consulta os eventos diretamente no Google Calendar utilizando as credenciais do usuário.
    /// </remarks>
    /// <param name="handler">Handler responsável pela execução da busca de eventos.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Lista de eventos do Google Calendar.</returns>
    /// <response code="200">Lista de eventos obtida com sucesso.</response>
    /// <response code="400">Parâmetros inválidos ou erro na requisição.</response>
    /// <response code="401">O usuário não está autenticado.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpGet("Calendar/Events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<List<GoogleCalendarEvent>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<List<GoogleCalendarEvent>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> GetGoogleCalendarEventsAsync(
        [FromServices] IHandlerBase<GetCalendarEventsQuery, List<GoogleCalendarEvent>> handler,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(new GetCalendarEventsQuery(), cancellationToken));
}