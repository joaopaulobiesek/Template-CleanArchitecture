namespace Template.Application.Domains.V1.ViewModels.Users;

public class LoginUserVm
{
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Modules { get; set; }
    public string Token { get; set; }

    public LoginUserVm() { }

    public LoginUserVm(string name, string email, List<string> modules, string token)
    {
        Name = name;
        Email = email;
        Modules = modules;
        Token = token;
    }
}