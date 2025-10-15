namespace Template.Domain.Constants;

public abstract class Roles
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);
    public const string Exhibitor = nameof(Exhibitor);
    public const string ServiceProvider = nameof(ServiceProvider);

    public static Dictionary<string, string> GetRoles()
    {
        return new Dictionary<string, string>
        {
           { nameof(Admin), Admin },
           { nameof(User), User },
           { nameof(Exhibitor), Exhibitor },
           { nameof(ServiceProvider), ServiceProvider }
        };
    }

    public static List<string> GetAllRoles()
    {
        return new List<string>
        {
            nameof(Admin),
            nameof(User),
            nameof(Exhibitor),
            nameof(ServiceProvider)
        };
    }
}