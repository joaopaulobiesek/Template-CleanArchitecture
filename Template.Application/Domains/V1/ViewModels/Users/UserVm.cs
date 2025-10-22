using Template.Application.Common.Interfaces.Security;

namespace Template.Application.Domains.V1.ViewModels.Users;

public class UserVm : IUser
{
    public string? Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public string? ProfileImageUrl { get; set; }
    public List<string>? Roles { get; set; }
    public List<string>? Policies { get; set; }

    public UserVm() { }
    public UserVm(string? id, string? email)
    {
        Id = id;
        Email = email;
    }

    public UserVm(string? id, string? email, string? fullName, string? profileImageUrl, List<string>? roles, List<string>? policies)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        ProfileImageUrl = profileImageUrl;
        Roles = roles;
        Policies = policies;
    }

    public UserVm(string? id, string? email, string? fullName, string? phoneNumber, string? profileImageUrl, List<string>? roles, List<string>? policies, bool phoneNumberConfirmed = false)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = phoneNumberConfirmed;
        ProfileImageUrl = profileImageUrl;
        Roles = roles;
        Policies = policies;
    }
}