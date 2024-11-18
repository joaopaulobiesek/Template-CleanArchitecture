namespace Template.Infra.Settings.Configurations;

public class JwtConfiguration
{
    public const string Key = "JwtSettings";
    public string? Secret { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int ExpiracaoEmMinutos { get; set; }
}

