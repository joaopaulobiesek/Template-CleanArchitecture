
namespace Template.Application.Common.Modules;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class RequiresModuleAttribute : Attribute
{
    public string Module { get; }

    public RequiresModuleAttribute(string module)
    {
        Module = module;
    }
}
