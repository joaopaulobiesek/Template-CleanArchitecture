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
    /// Responsável por criar um novo usuário no sistema.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite a criação de um novo usuário com suas respectivas permissões e funções.
    /// 
    /// **Regras de negócio:**
    /// - O usuário deve possuir um e-mail e uma senha válidos.
    /// - As funções e permissões devem ser definidas no momento da criação.
    /// </remarks>
    /// <param name="handler">Handler responsável pelo processamento da criação do usuário.</param>
    /// <param name="command">Os parâmetros necessários para criar o usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Os detalhes do usuário criado.</returns>
    /// <response code="200">Usuário criado com sucesso.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    /// <response code="401">O usuário não está autenticado para acessar este recurso.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<User>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<User>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> CreateUserAsync(
        [FromServices] IHandlerBase<CreateUserCommand, User> handler,
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(command, cancellationToken));

    /// <summary>
    /// Obtém a lista de políticas de acesso do sistema.
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna a lista de políticas disponíveis para configuração de permissões.
    /// </remarks>
    /// <param name="handler">Handler responsável pela consulta das políticas.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Lista de políticas disponíveis.</returns>
    /// <response code="200">Lista de políticas obtida com sucesso.</response>
    /// <response code="401">O usuário não está autenticado para acessar este recurso.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpGet("Polices")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<IEnumerable<KeyValuePairVM>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> PolicesAsync(
        [FromServices] IHandlerBase<GetPolicesQuery, IEnumerable<KeyValuePairVM>> handler,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(new GetPolicesQuery(), cancellationToken));

    /// <summary>
    /// Obtém a lista de funções disponíveis no sistema.
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna todas as funções disponíveis para atribuição de permissões.
    /// </remarks>
    /// <param name="handler">Handler responsável pela consulta das funções.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Lista de funções disponíveis.</returns>
    /// <response code="200">Lista de funções obtida com sucesso.</response>
    /// <response code="401">O usuário não está autenticado para acessar este recurso.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpGet("Roles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<IEnumerable<KeyValuePairVM>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> RolesAsync(
        [FromServices] IHandlerBase<GetRolesQuery, IEnumerable<KeyValuePairVM>> handler,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(new GetRolesQuery(), cancellationToken));

    /// <summary>
    /// Atualiza as informações de um usuário.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite a edição dos detalhes do usuário, incluindo permissões e funções.
    /// </remarks>
    /// <param name="handler">Handler responsável pelo processamento da edição do usuário.</param>
    /// <param name="command">Os parâmetros necessários para editar o usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Os detalhes do usuário atualizado.</returns>
    /// <response code="200">Usuário atualizado com sucesso.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    /// <response code="401">O usuário não está autenticado para acessar este recurso.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<User>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<User>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> EditUserAsync(
        [FromServices] IHandlerBase<EditUserCommand, User> handler,
        [FromBody] EditUserCommand command,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(command, cancellationToken));

    /// <summary>
    /// Lista usuários paginados e filtrados.
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna uma lista paginada de usuários, permitindo filtro por e-mail, CPF ou nome.
    /// </remarks>
    /// <param name="handler">Handler responsável pelo processamento da consulta.</param>
    /// <param name="query">Os parâmetros necessários para a listagem de usuários.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Lista paginada de usuários.</returns>
    /// <response code="200">Lista de usuários obtida com sucesso.</response>
    /// <response code="401">O usuário não está autenticado para acessar este recurso.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<UserVm>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> UsuariosAsync(
        [FromServices] IHandlerBase<GetAllQuery, IEnumerable<UserVm>> handler,
        [FromQuery] GetAllQuery query,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(query, cancellationToken));

    /// <summary>
    /// Deleta um usuário pelo ID.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite a exclusão de um usuário com base em seu identificador único.
    /// </remarks>
    /// <param name="handler">Handler responsável pelo processamento da exclusão do usuário.</param>
    /// <param name="userId">O identificador do usuário a ser deletado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Confirmação da exclusão do usuário.</returns>
    /// <response code="200">Usuário deletado com sucesso.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    /// <response code="401">O usuário não está autenticado para acessar este recurso.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> DeleteUserAsync(
       [FromServices] IHandlerBase<Guid, string> handler,
       Guid userId, CancellationToken cancellationToken)
       => HandleResponse(await handler.Execute(userId, cancellationToken));
}