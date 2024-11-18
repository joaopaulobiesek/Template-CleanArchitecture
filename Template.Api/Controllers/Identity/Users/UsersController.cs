using Template.Api.Controllers.System;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Models;
using Template.Application.Domains.Identity.Users.Commands.CreateUsers;
using Template.Application.Domains.Identity.Users.Commands.EditUsers;
using Template.Application.Domains.Identity.Users.Queries.GetAll;
using Template.Application.Domains.Identity.Users.Queries.GetPolices;
using Template.Application.Domains.Identity.Users.Queries.GetRoles;
using Template.Application.ViewModels.Shared;
using Template.Application.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;

namespace Template.Api.Controllers.Identity.Users;

[Route("[controller]")]
public class UsersController : BaseController
{
    /// <summary>
    /// Responsável por criar novos usuários no sistema.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="command">Forneça as informações para criar o usuário.</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<User>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<User>))]
    public async Task<IActionResult> CreateUserAsync(
        [FromServices] IHandlerBase<CreateUserCommand, User> handler,
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(command, cancellationToken));


    /// <summary>
    /// Obtém a lista completa de políticas de acesso do sistema para exibição e configuração de permissões.
    /// </summary>
    /// <param name="handler">Handler que executa a consulta para obter as políticas.</param>
    /// <param name="cancellationToken">Token de cancelamento para interromper a operação, se necessário.</param>
    /// <returns>Retorna uma lista de objetos KeyValuePairVM, contendo as políticas de acesso.</returns>
    [HttpGet("Polices")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<IEnumerable<KeyValuePairVM>>))]
    public async Task<IActionResult> PolicesAsync(
            [FromServices] IHandlerBase<GetPolicesQuery, IEnumerable<KeyValuePairVM>> handler,
            CancellationToken cancellationToken)
            => HandleResponse(await handler.Execute(new GetPolicesQuery(), cancellationToken));

    /// <summary>
    /// Obtém a lista completa de funções disponíveis no sistema para definição de permissões e acesso.
    /// </summary>
    /// <param name="handler">Handler que executa a consulta para obter as roles.</param>
    /// <param name="cancellationToken">Token de cancelamento para interromper a operação, se necessário.</param>
    /// <returns>Retorna uma lista de objetos KeyValuePairVM, contendo as funções disponíveis no sistema.</returns>
    [HttpGet("Roles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<IEnumerable<KeyValuePairVM>>))]
    public async Task<IActionResult> RolesAsync(
            [FromServices] IHandlerBase<GetRolesQuery, IEnumerable<KeyValuePairVM>> handler,
            CancellationToken cancellationToken)
            => HandleResponse(await handler.Execute(new GetRolesQuery(), cancellationToken));

    /// <summary>
    /// Responsável por editar usuário no sistema.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="command">Forneça as informações para editar o usuário.</param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<User>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<User>))]
    public async Task<IActionResult> EditUserAsync(
        [FromServices] IHandlerBase<EditUserCommand, User> handler,
        [FromBody] EditUserCommand command, CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(command, cancellationToken));

    /// <summary>
    /// Responsável por listar usuários paginados, ordenados e filtrados por email, CPF ou nome.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="query">O objeto de consulta contendo os parâmetros de paginação, ordenação e filtro.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<UserVm>))]
    public async Task<IActionResult> UsuariosAsync(
        [FromServices] IHandlerBase<GetAllQuery, IEnumerable<UserVm>> handler,
        [FromQuery] GetAllQuery query,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(query, cancellationToken));

    /// <summary>
    /// Responsável por deletar usuário pelo Id.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> DeleteUserAsync(
        [FromServices] IHandlerBase<Guid, string> handler,
        Guid userId, CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(userId, cancellationToken));
}