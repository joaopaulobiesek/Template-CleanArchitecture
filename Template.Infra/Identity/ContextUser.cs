using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;

namespace Template.Infra.Identity;

public class ContextUserClaim : IdentityUserClaim<string>
{

}

public class ContextUserLogin : IdentityUserLogin<string>
{

}

public class ContextUserToken : IdentityUserToken<string>
{

}

public class ContextUserRole : IdentityUserRole<string>
{
}

public class ContextRoleClaim : IdentityRoleClaim<string>
{

}

public class ContextRole : IdentityRole<string>
{

    public ContextRole(string name)
    {
        Name = name;
        Id = Guid.NewGuid().ToString();
    }
}

public class ContextUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? ProfileImageUrl { get; set; }

    public ContextUser()
    {
    }

    public ContextUser(string fullName)
    {
        FullName = fullName;
    }

    public void Update(IUser u)
    {
        if (!string.IsNullOrWhiteSpace(u.FullName) && FullName != u.FullName)
            FullName = u.FullName;

        if (ProfileImageUrl != u.ProfileImageUrl && !string.IsNullOrWhiteSpace(u.ProfileImageUrl))
            ProfileImageUrl = u.ProfileImageUrl;
    }

    public static implicit operator ContextUser(User u)
    => new(fullName: u.FullName!)
    {
        Id = u.Id,
        ProfileImageUrl = u.ProfileImageUrl
    };
}