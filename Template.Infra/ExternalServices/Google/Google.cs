using Google.Apis.Auth;
using System.Text.Json;
using System.Text.Json.Serialization;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;
using Template.Infra.Settings.Configurations;

namespace Template.Infra.ExternalServices.Google
{
    /// <summary>
    /// Serviço para integração com o Google OAuth 2.0 e autenticação de usuários.
    /// </summary>
    internal class Google : IGoogle
    {
        private readonly bool _isActive;
        private readonly string _scope;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _googleAccountUrl;
        private readonly string _googleApiUrl;
        private readonly string _googleCalendarUrl;
        private readonly string _redirectUrl;
        private readonly string _provider;
        private readonly string _tokenName;

        /// <summary>
        /// Inicializa uma nova instância do serviço de autenticação Google.
        /// </summary>
        /// <param name="google">Configuração do Google fornecida pelo sistema.</param>
        public Google(GoogleConfiguration google)
        {
            _isActive = google.Active!;
            _scope = google.Scope!;
            _clientId = google.ClientId!;
            _clientSecret = google.ClientSecret!;
            _redirectUrl = google.RedirectUri!;
            _googleApiUrl = google.UrlGoogleAPI!;
            _googleAccountUrl = google.UrlGoogleAccount!;
            _googleCalendarUrl = google.UrlGoogleCalendar!;
            _provider = "Google";
            _tokenName = "AccessToken";
        }

        /// <summary>
        /// Autentica o usuário usando o Google e associa o login externo ao sistema.
        /// </summary>
        /// <param name="identity">Serviço de identidade para gerenciamento do login externo.</param>
        /// <param name="code">Código de autorização recebido do Google após o redirecionamento.</param>
        /// <returns>Resultado indicando sucesso ou erro durante o processo de autenticação.</returns>
        public async Task<ApiResponse<string>> AuthenticateUserAsync(IIdentityService identity, string code, Guid xTenantID)
        {
            if (!_isActive)
                return new ErrorResponse<string>("Authentication service is disabled.");

            if (string.IsNullOrEmpty(code))
                return new ErrorResponse<string>("Authorization code not received.");

            var token = await ExchangeAuthorizationCodeForTokenAsync(code);

            if (token == null)
                return new ErrorResponse<string>("Failed to generate Google token.");

            var userPayload = await ValidateTokenAsync(token.IdToken);

            await identity.RemoveLoginProviderTokenAsync(
                userPayload.JwtId,
                _provider,
                _tokenName
            );

            await identity.AddLoginProviderTokenAsync(
                userPayload.JwtId,
                _provider,
                _tokenName,
                token.AccessToken
            );

            return await identity.HandleExternalLoginAsync(
                _provider,
                userPayload.JwtId,
                userPayload.Email,
                userPayload.Name,
                userPayload.Picture, 
                xTenantID
            );
        }

        /// <summary>
        /// Gera a URL para iniciar o fluxo de autenticação OAuth do Google.
        /// </summary>
        /// <param name="state">Um estado opcional para segurança adicional.</param>
        /// <returns>A URL que deve ser acessada pelo cliente para autenticar.</returns>
        public ApiResponse<string> GenerateAuthenticationUrl(string? state = null)
        {
            if (!_isActive)
                return new ErrorResponse<string>("Authentication service is disabled.");

            var queryParams = new[]
            {
                $"client_id={_clientId}",
                $"redirect_uri={Uri.EscapeDataString(_redirectUrl)}",
                "response_type=code",
                _scope,
                state != null ? $"state={state}" : null
            };

            return new SuccessResponse<string>("Generate authentication URL successfully!", _googleAccountUrl + string.Join("&", queryParams));
        }

        /// <summary>
        /// Recupera os eventos do Google Calendar para o usuário autenticado.
        /// </summary>
        /// <param name="identity">Serviço de identidade para gerenciamento do usuário.</param>
        /// <param name="userId">ID do usuário autenticado no sistema.</param>
        /// <returns>Uma lista de eventos do Google Calendar.</returns>
        public async Task<ApiResponse<List<GoogleCalendarEvent>>> GetGoogleCalendarEventsAsync(IIdentityService identity, string userId)
        {
            var accessToken = await identity.GetLoginProviderTokenAsync(userId, _provider, _tokenName);

            if (!accessToken.Success)
                return new ErrorResponse<List<GoogleCalendarEvent>>("Access token is required.");

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Data);

                var response = await httpClient.GetAsync(_googleCalendarUrl);

                if (!response.IsSuccessStatusCode)
                    return new ErrorResponse<List<GoogleCalendarEvent>>($"Failed to fetch Google Calendar events. Error: {response.StatusCode}", (int)response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var events = JsonSerializer.Deserialize<GoogleCalendarEventsResponse>(content);

                return new SuccessResponse<List<GoogleCalendarEvent>>("Successfully fetched Google Calendar events.", events?.Items ?? new List<GoogleCalendarEvent>());
            }
            catch (Exception ex)
            {
                return new ErrorResponse<List<GoogleCalendarEvent>>($"An error occurred while fetching Google Calendar events: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida o ID Token do Google e retorna as informações do usuário autenticado.
        /// </summary>
        /// <param name="idToken">O token JWT recebido após a autenticação.</param>
        /// <returns>Informações do usuário autenticado.</returns>
        private async Task<GoogleJsonWebSignature.Payload> ValidateTokenAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                payload.JwtId ??= payload.Subject;

                return payload;

            }
            catch (Exception ex)
            {
                throw new Exception("Error validating Google token: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Troca o código de autorização pelo token de acesso e ID token.
        /// </summary>
        /// <param name="code">Código de autorização recebido após o redirecionamento.</param>
        /// <returns>Um objeto contendo o ID Token e o Access Token.</returns>
        private async Task<TokenResponse?> ExchangeAuthorizationCodeForTokenAsync(string code)
        {
            using var httpClient = new HttpClient();

            var payload = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("redirect_uri", _redirectUrl),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
            });

            var response = await httpClient.PostAsync(_googleApiUrl, payload);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error exchanging authorization code for token: " + response.Content);

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TokenResponse>(json);
        }

        /// <summary>
        /// Representa a resposta do Google ao trocar o código de autorização por tokens.
        /// </summary>
        private class TokenResponse
        {
            /// <summary>
            /// Token de acesso para chamadas às APIs do Google.
            /// </summary>
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            /// <summary>
            /// ID Token contendo informações do usuário autenticado.
            /// </summary>
            [JsonPropertyName("id_token")]
            public string IdToken { get; set; }

            /// <summary>
            /// Tempo de expiração do token, em segundos.
            /// </summary>
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            /// <summary>
            /// Escopos concedidos ao token.
            /// </summary>
            [JsonPropertyName("scope")]
            public string Scope { get; set; }

            /// <summary>
            /// Tipo de token gerado (geralmente "Bearer").
            /// </summary>
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
        }
    }
}