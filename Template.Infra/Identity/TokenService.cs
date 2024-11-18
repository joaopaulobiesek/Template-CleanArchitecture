using Template.Infra.Settings.Configurations;
using Microsoft.Extensions.Options;

namespace Template.Infra.Identity;

public class TokenService : ITokenService
{
    private readonly JwtConfiguration _jwtConfig;

    public TokenService(IOptions<JwtConfiguration> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
    }

    /// <summary>
    /// Gera um token JWT criptografado com as credenciais e claims fornecidas.
    /// </summary>
    public string GenerateJwt(ContextUser user, List<string> roles)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = GetClaims(user, roles);
        return GenerateEncryptedToken(signingCredentials, claims);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           issuer: _jwtConfig.Issuer, // Define o Issuer do token
           audience: _jwtConfig.Audience, // Define o Audience do token
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiracaoEmMinutos),
           signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secretKey = Encoding.ASCII.GetBytes(_jwtConfig.Secret ?? throw new ArgumentNullException(nameof(_jwtConfig.Secret)));
        return new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);
    }

    /// <summary>
    /// Cria uma lista de claims para o usuário fornecido, usando tipos padrão para integração com o ASP.NET Core.
    /// </summary>
    private static IEnumerable<Claim> GetClaims(ContextUser user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id), // Claim padrão para o ID do usuário
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty), // Claim padrão para o email
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty), // Claim padrão para o nome
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty) // Claim padrão para telefone
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    /// <summary>
    /// Valida o token JWT fornecido e retorna o ID do usuário, se válido.
    /// </summary>
    public string ValidateTokenGetUserId(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return "Invalid token";
        }

        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token["Bearer ".Length..];
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "User ID not found";
        }
        catch (SecurityTokenException ex)
        {
            // Log this exception as needed
            return $"Token validation failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            // Log this exception as needed
            return $"Unexpected error: {ex.Message}";
        }
    }

    /// <summary>
    /// Define os parâmetros de validação do token.
    /// </summary>
    private TokenValidationParameters GetValidationParameters()
    {
        var secretKey = Encoding.ASCII.GetBytes(_jwtConfig.Secret ?? throw new ArgumentNullException(nameof(_jwtConfig.Secret)));

        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _jwtConfig.Issuer,
            ValidAudience = _jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ClockSkew = TimeSpan.Zero // Remove a tolerância de atraso padrão de 5 minutos
        };
    }
}