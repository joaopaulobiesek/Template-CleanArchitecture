using Microsoft.AspNetCore.Mvc;
using Template.Api.Controllers.System;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Models;
using Template.Application.Domains.Tenant.V1.ViewModels;
using Template.Application.Domains.Tenant.V1.Clients.Commands.CreateClient;
using Template.Application.Domains.Tenant.V1.Clients.Commands.DeactivateClient;
using Template.Application.Domains.Tenant.V1.Clients.Commands.UpdateClient;
using Template.Application.Domains.Tenant.V1.Clients.Queries.GetAll;

namespace Template.Api.Controllers.Tenant.V1.Clients;

[ApiController]
[Route("tenant/api/v1/[controller]")]
[ApiExplorerSettings(GroupName = "Tenant.Api.v1")]
public class ClientController : BaseController
{
    /// <summary>
    /// Responsável por Registrar um novo cliente.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<ClientVM>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<ClientVM>))]
    public async Task<IActionResult> CreateClientAsync(
        [FromServices] IHandlerBase<CreateClientCommand, ClientVM> handler,
        [FromBody] CreateClientCommand command,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(command, cancellationToken));

    /// <summary>
    /// Responsável por Editar um cliente.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<ClientVM>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<ClientVM>))]
    public async Task<IActionResult> UpdateClientAsync(
        [FromServices] IHandlerBase<UpdateClientCommand, ClientVM> handler,
        [FromBody] UpdateClientCommand command,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(command, cancellationToken));

    /// <summary>
    /// Responsável por listar todos os clientes.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<ClientVM>))]
    public async Task<IActionResult> GetAllAsync(
        [FromServices] IHandlerBase<GetAllQuery, IEnumerable<ClientVM>> handler,
        [FromQuery] GetAllQuery query,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(query, cancellationToken));

    /// <summary>
    /// Responsável por desativar um cliente.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("Deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> DeactivateClientAsync(
        [FromServices] IHandlerBase<DeactivateClientCommand, string> handler,
        [FromBody] DeactivateClientCommand command, CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(command, cancellationToken));

    /// <summary>
    /// Responsável por excluir um cliente fisicamente.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="clientId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("Delete/{clientId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> DeleteClientAsync(
        [FromServices] IHandlerBase<Guid, string> handler,
        Guid clientId, CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(clientId, cancellationToken));
}