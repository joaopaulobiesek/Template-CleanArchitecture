using Microsoft.AspNetCore.Mvc;
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
[Route("[controller]")]
public class AuthController : BaseController
{
    /// <summary>
    /// Realiza o login no sistema utilizando as credenciais fornecidas pelo usuário.
    /// </summary>
    /// <param name="handler">Handler para processar a execução do comando de login.</param>
    /// <param name="command">Comando contendo as credenciais de login do usuário.</param>
    /// <param name="cancellationToken">Token para controle de cancelamento da operação.</param>
    /// <returns>Resposta indicando sucesso ou falha na autenticação, incluindo o token em caso de sucesso.</returns>
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
    /// Processa o retorno do Google OAuth 2.0 após a autenticação, gerando um token para o usuário no sistema.
    /// </summary>
    /// <param name="google">Serviço de autenticação Google responsável pelo processamento do login externo.</param>
    /// <param name="service">Serviço de identidade para manipulação de usuários e integrações.</param>
    /// <param name="code">Código de autorização recebido do Google após o login.</param>
    /// <param name="cancellationToken">Token para controle de cancelamento da operação.</param>
    /// <returns>Resposta contendo sucesso ou falha no processo de autenticação, com os detalhes necessários.</returns>
    [HttpGet("Google/Callback")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> GoogleCallback(
        [FromServices] IGoogle google, [FromServices] IIdentityService service,
         string code, CancellationToken cancellationToken)
        => HandleResponse(await google.AuthenticateUserAsync(service, code));

    /// <summary>
    /// Gera o link de login para iniciar o processo de autenticação no Google.
    /// </summary>
    /// <param name="google">Serviço de autenticação Google responsável por gerar o URL de autenticação.</param>
    /// <param name="cancellationToken">Token para controle de cancelamento da operação.</param>
    /// <returns>Resposta contendo a URL gerada para o login no Google.</returns>
    [HttpGet("Google")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> GoogleLogin(
        [FromServices] IGoogle google, CancellationToken cancellationToken)
        => HandleResponse(google.GenerateAuthenticationUrl());
}