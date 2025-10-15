
namespace Template.Domain.Common.Tenant;

public static class Modules
{
    public const string Booth = "Booth";

    public static IReadOnlyList<string> GetAllModules()
        => new[] { Booth, };
}
