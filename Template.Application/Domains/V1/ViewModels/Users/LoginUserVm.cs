namespace Template.Application.Domains.V1.ViewModels.Users;

public class LoginUserVm
{
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Modules { get; set; }
    public List<string> Roles { get; set; }
    public List<string> Policies { get; set; }
    public string Token { get; set; }

    public LoginUserVm() { }

    public LoginUserVm(string name, string email, List<string> modules, List<string> roles, List<string> policies, string token)
    {
        Name = name;
        Email = email;
        Modules = modules;
        Roles = roles;
        Policies = policies;
        Token = token;
    }
}