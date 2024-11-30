namespace Template.Application.ViewModels.Users;

public class LoginUserVm
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }

    public LoginUserVm() { }

    public LoginUserVm(string name, string email, string token)
    {
        Name = name;
        Email = email;
        Token = token;
    }
}