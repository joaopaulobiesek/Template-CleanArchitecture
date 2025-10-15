
namespace Template.Domain.Common.Tenant;

public static class ModuleActionType
{
    public const string Activate = "Activate";
    public const string Deactivate = "Deactivate";

     /// <summary>
    /// Retorna todos os tipos de campos suportados.
    /// </summary>
    public static IReadOnlyList<string> GetAllTypes()
    {
        return new List<string>
        {
            Activate,
            Deactivate
        };
    }
}