namespace Template.Infra.Settings.Configurations;

public class StorageConfiguration
{
    public const string Key = "Storage";
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}