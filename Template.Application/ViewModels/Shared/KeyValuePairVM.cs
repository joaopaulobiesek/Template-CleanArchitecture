namespace Template.Application.ViewModels.Shared;

public class KeyValuePairVM
{
    public string Key { get; set; }
    public string Value { get; set; }

    public KeyValuePairVM() { }

    public KeyValuePairVM(string key, string value)
    {
        Key = key;
        Value = value;
    }
}