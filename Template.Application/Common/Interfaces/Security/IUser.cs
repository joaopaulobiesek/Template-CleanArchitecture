namespace Template.Application.Common.Interfaces.Security;

public interface IUser
{
    public string? Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }
    public List<string>? Roles { get; set; }
    public List<string>? Policies { get; set; }
}