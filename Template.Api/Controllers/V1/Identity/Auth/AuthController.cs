using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Template.Api.Controllers.System;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;
using Template.Application.Domains.V1.Identity.Auth.Commands.LoginUser;
using Template.Application.Domains.V1.Identity.Auth.Commands.Register;
using Template.Application.Domains.V1.ViewModels.Users;
using Template.Infra.Settings.Configurations;

namespace Template.Api.Controllers.V1.Identity.Auth;

/// <summary>
/// Controller responsável pelas operações de autenticação, incluindo login no sistema e integração com provedores externos.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : BaseController
{
    private readonly JwtConfiguration _jwtConfig;

    public AuthController(IOptions<JwtConfiguration> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
    }
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
        {
            // Define cookie HTTP-Only com o token JWT
            SetAuthCookie(response.Data.Token);
            return Ok(response);
        }
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<LoginUserVm>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> GoogleCallback(
        [FromServices] IGoogle google,
        [FromServices] IIdentityService service,
        string code,
        string? state,
        CancellationToken cancellationToken)
    {
        var result = await google.AuthenticateUserAsync(service, code, state);

        // Se autenticação bem-sucedida, define cookie HTTP-Only
        if (result.Success && !string.IsNullOrEmpty(result.Data?.Token))
        {
            SetAuthCookie(result.Data.Token);
        }

        return HandleResponse(result);
    }

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
        => HandleResponse(google.GenerateAuthenticationUrl(Guid.Empty.ToString()));

    /// <summary>
    /// Registra um novo usuário usando um código de convite.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite que novos usuários se cadastrem no sistema usando um código de convite válido.
    ///
    /// **Regras de negócio:**
    /// - O código de convite deve existir, estar ativo e não ter expirado.
    /// - O código não pode ter atingido o número máximo de usos.
    /// - O usuário é criado com a role "USER" automaticamente.
    /// - Após o registro, o usuário é automaticamente autenticado e recebe um token JWT.
    /// - O código de convite tem seu contador de usos incrementado.
    /// </remarks>
    /// <param name="handler">Handler responsável pelo processamento do registro.</param>
    /// <param name="command">Os parâmetros necessários para registro (nome, email, telefone, senha e código de convite).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Os detalhes do usuário autenticado com o token gerado.</returns>
    /// <response code="200">Usuário registrado e autenticado com sucesso.</response>
    /// <response code="400">Parâmetros inválidos ou código de convite inválido.</response>
    /// <response code="404">Código de convite não encontrado.</response>
    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<LoginUserVm>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> RegisterAsync(
        [FromServices] IHandlerBase<RegisterCommand, LoginUserVm> handler,
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var response = await handler.Execute(command, cancellationToken);

        if (!response.Success)
        {
            if (response.StatusCode == 404)
                return NotFound(response);

            return BadRequest(response);
        }

        // Define cookie HTTP-Only após registro bem-sucedido
        if (!string.IsNullOrEmpty(response.Data?.Token))
        {
            SetAuthCookie(response.Data.Token);
        }

        return Ok(response);
    }

    /// <summary>
    /// Realiza o logout do usuário, removendo o cookie de autenticação.
    /// </summary>
    /// <remarks>
    /// Este endpoint remove o cookie HTTP-Only que contém o token JWT, efetivamente deslogando o usuário.
    ///
    /// **Regras de negócio:**
    /// - O usuário deve estar autenticado para fazer logout.
    /// - O cookie é expirado imediatamente (data no passado).
    /// - O token JWT continua tecnicamente válido até expirar, mas o navegador não o envia mais.
    /// </remarks>
    /// <returns>Mensagem de sucesso.</returns>
    /// <response code="200">Logout realizado com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpPost("Logout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        // Remove o cookie de autenticação
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        Response.Cookies.Delete("auth_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = isDevelopment ? SameSiteMode.None : SameSiteMode.Lax,             // Consistente com SetAuthCookie
            Path = "/"                                // Mesmo Path usado no Set
        });

        return Ok(new SuccessResponse<string>("Logout realizado com sucesso"));
    }

    /// <summary>
    /// Define o cookie HTTP-Only para autenticação JWT.
    /// </summary>
    private void SetAuthCookie(string token)
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,                          // JavaScript não consegue acessar (proteção XSS)
            Secure = true,                            // Apenas HTTPS em produção
            SameSite = isDevelopment ? SameSiteMode.None : SameSiteMode.Lax,             // Funciona em modo anônimo (recomendação Google/Facebook)
            Path = "/",                               // Cookie enviado em todas as rotas
            MaxAge = TimeSpan.FromMinutes(_jwtConfig.ExpiracaoEmMinutos)  // Mais confiável que Expires
        };

        Response.Cookies.Append("auth_token", token, cookieOptions);
    }
}