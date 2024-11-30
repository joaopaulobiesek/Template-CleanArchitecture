namespace Template.Infra.Settings.Configurations;

public class SendGridConfiguration
{
    public const string Key = "SendGrid";
    public string API_KEY { get; set; }
    public string[]? EmailDocumento { get; set; }
}