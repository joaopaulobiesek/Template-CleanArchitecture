namespace Template.Infra.Settings.Configurations;

public class GoogleConfiguration
{
    public const string Key = "Authentication";
    public const string GoogleKey = "Google";
    public bool Active { get; set; }
    public string? RedirectUri { get; set; }
    public string? UrlGoogleAPI { get; set; }
    public string? UrlGoogleAccount { get; set; }
    public string? UrlGoogleCalendar { get; set; }
    public string? Scope { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}