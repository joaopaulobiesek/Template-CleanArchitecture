using Microsoft.AspNetCore.Mvc;
using Template.Api.Controllers.System;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;
using Template.Application.Domains.V1.Identity.Auth.Commands.LoginUser;
using Template.Application.Domains.V1.ViewModels.Users;

namespace Template.Api.Controllers.V1.Identity.Auth;

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
    /// `Tenant ID:` 94889bac-1b30-485d-a0c6-df1faaf9e7ce
    /// 
    /// `Token ADMIN:` Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjVjZWYyZmUzLTJiNjItNGJlZi05YzMyLWRiMzUyYTkyNzgyMCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGFkbWluLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJBZG1pbmlzdHJhZG9yIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBRE1JTiIsImV4cCI6MTc2NjE1NDYxMSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3QiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdCJ9.X_9oSbC3RxCrKWJ_0a8n4xSToVi9lA7bscZE4ELa-4M
    ///
    /// 
    /// `Token Expositor:` Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjQxNjI0OGJlLWM2ZTgtNDc3Yi05OTg5LTdhZWYyYTZiZjUxZCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImV4cG9zaXRvckBhZG1pbi5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiRXhwb3NpdG9yIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJFWEhJQklUT1IiLCJleHAiOjE3NjYxNTQ3NzYsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0IiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3QifQ.G_N5qFmQNevcRyUafqu96BaQnPT2zJxjsjFwj-q27zw
    ///  
    /// 
    /// `PrestadorDeServico:` Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImQ0ZWUxZjIxLWMyZmMtNGZjMS1hMDIxLTdkYjg4MTg1NWRlMCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6Im1vbnRhZG9yYUBhZG1pbi5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiTW9udGFkb3JhIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJTRVJWSUNFUFJPVklERVIiLCJleHAiOjE3NjYxNTQ4MDIsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0IiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3QifQ.EhOpwVMg1OU_v14jYpq1HEME2N_Lk79WsF6kAtUrkeQ
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<LoginUserVm>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync(
        [FromServices] IHandlerBase<LoginUserCommand, LoginUserVm> handler,
        [FromBody] LoginUserCommand command,
        CancellationToken cancellationToken)
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> GoogleCallback(
        [FromServices] IGoogle google,
        [FromServices] IIdentityService service,
        string code,
        string? state,
        CancellationToken cancellationToken)
        => HandleResponse(await google.AuthenticateUserAsync(service, code, ResolveTenantId(state)));

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
        [FromServices] IGoogle google,
        CancellationToken cancellationToken)
        => HandleResponse(google.GenerateAuthenticationUrl(ResolveTenantId(null).ToString()));

    /// <summary>
    /// Determina o Tenant ID a partir do parâmetro 'state' ou do header 'X-Tenant-ID'.
    /// </summary>
    /// <param name="state">Parâmetro de estado opcional contendo o Tenant ID.</param>
    /// <returns>O Tenant ID como um Guid ou Guid.Empty se não encontrado.</returns>
    private Guid ResolveTenantId(string? state)
    {
        if (!string.IsNullOrEmpty(state) && Guid.TryParse(state, out var stateTenantId))
        {
            return stateTenantId;
        }

        var tenantIdHeader = HttpContext?.Request.Headers["X-Tenant-ID"].ToString();
        if (!string.IsNullOrEmpty(tenantIdHeader) && Guid.TryParse(tenantIdHeader, out var headerTenantId))
        {
            return headerTenantId;
        }

        return Guid.Empty;
    }
}