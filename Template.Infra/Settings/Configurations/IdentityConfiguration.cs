namespace Template.Infra.Settings.Configurations;

public class IdentityConfiguration
{
    public const string IdentityKey = "Identity";
    public string? EmailAdmin { get; set; }
    public string? SenhaAdmin { get; set; }
    public string? RoleAdmin { get; set; }
    public string[]? Roles { get; set; }
}