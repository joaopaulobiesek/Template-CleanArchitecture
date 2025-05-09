﻿using Microsoft.AspNetCore.Mvc;
using Template.Api.Controllers.System;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;
using Template.Application.Domains.Identity.Auth.Commands.LoginUser;
using Template.Application.ViewModels.Users;

namespace Template.Api.Controllers.Identity.Auth;

/// <summary>
/// Controller responsável pelas operações de autenticação, incluindo login no sistema e integração com provedores externos.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : BaseController
{
    /// <summary>
    /// Realiza a autenticação do usuário no sistema.
    /// </summary>
    /// <remarks>
    /// Este endpoint processa o login do usuário no sistema, validando suas credenciais.
    /// 
    /// **Regras de negócio:**
    /// - O usuário deve fornecer um e-mail e senha válidos.
    /// - O sistema retorna um token JWT caso a autenticação seja bem-sucedida.
    /// - Caso o token não seja gerado corretamente, retorna erro de autenticação.
    ///     
    /// </remarks>
    /// <param name="handler">Handler responsável pelo processamento da autenticação.</param>
    /// <param name="command">Os parâmetros necessários para login do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Os detalhes do usuário autenticado com o token gerado.</returns>
    /// <response code="200">Login realizado com sucesso.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    /// <response code="401">Usuário ou senha inválidos.</response>
    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromServices] IHandlerBase<LoginUserCommand, LoginUserVm> handler, [FromBody] LoginUserCommand command, CancellationToken cancellationToken)
    {
        var response = await handler.Execute(command, cancellationToken);

        if (!response.Success)
            return BadRequest(response);
        else if (string.IsNullOrEmpty(response.Data!.Token))
            return Unauthorized();
        else
            return Ok(response);
    }

    /// <summary>
    /// Processa o retorno do Google OAuth 2.0 após a autenticação.
    /// </summary>
    /// <remarks>
    /// Este endpoint recebe o código de autorização do Google e realiza a autenticação do usuário.
    /// 
    /// **Regras de negócio:**
    /// - O código de autorização deve ser válido.
    /// - Se a autenticação for bem-sucedida, o sistema gera um token JWT para o usuário.
    /// </remarks>
    /// <param name="google">Serviço de autenticação Google.</param>
    /// <param name="service">Serviço de identidade para manipulação de usuários.</param>
    /// <param name="code">Código de autorização do Google.</param>
    /// <param name="state">Estado retornado pela autenticação.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Resposta contendo sucesso ou falha na autenticação.</returns>
    /// <response code="200">Usuário autenticado com sucesso.</response>
    /// <response code="400">Código de autorização inválido.</response>
    /// <response code="401">Falha na autenticação do usuário.</response>
    [HttpGet("Google/Callback")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> GoogleCallback(
        [FromServices] IGoogle google, [FromServices] IIdentityService service,
         string code, CancellationToken cancellationToken)
        => HandleResponse(await google.AuthenticateUserAsync(service, code));

    /// <summary>
    /// Gera o link de login para autenticação no Google.
    /// </summary>
    /// <remarks>
    /// Este endpoint gera a URL necessária para iniciar o processo de autenticação com o Google.
    /// 
    /// **Regras de negócio:**
    /// - O sistema retorna a URL que deve ser acessada pelo cliente para autenticação.
    /// </remarks>
    /// <param name="google">Serviço de autenticação Google.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>URL de autenticação no Google.</returns>
    /// <response code="200">URL gerada com sucesso.</response>
    /// <response code="400">Erro ao gerar URL de autenticação.</response>
    [HttpGet("Google")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> GoogleLogin(
        [FromServices] IGoogle google, CancellationToken cancellationToken)
        => HandleResponse(google.GenerateAuthenticationUrl());
}