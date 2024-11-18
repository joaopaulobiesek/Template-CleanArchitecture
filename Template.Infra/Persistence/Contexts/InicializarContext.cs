using Template.Domain.Constants;
using Template.Infra.Settings.Configurations;

namespace Template.Infra.Persistence.Contexts;

public static class InitializerExtension
{
    public static async Task InitializeDatabaseAsync(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateAsyncScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.SeedAsync();
    }
}

public class DatabaseInitializer
{
    private readonly UserManager<ContextUser> _userManager;
    private readonly RoleManager<ContextRole> _roleManager;
    private readonly Context _context;
    private IdentityConfiguration _config;

    public DatabaseInitializer(
        UserManager<ContextUser> userManager,
        RoleManager<ContextRole> roleManager,
        Context context,
        IConfiguration configuration)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _config = configuration
            .GetSection(IdentityConfiguration.IdentityKey)
            .Get<IdentityConfiguration>()!;
    }

    public async Task SeedAsync()
    {
        await AddedRolesAsync();
        await AddedPoliciesAsync();
        await AddedUserAsync();
    }

    private async Task AddedUserAsync()
    {
        var admin = new ContextUser("Administrador")
        {
            Email = "admin@admin.com",
            UserName = "admin"
        };

        var usuarioCadastrado = await _userManager.FindByEmailAsync(admin.Email!);

        if (usuarioCadastrado is null)
        {
            await _userManager.CreateAsync(admin, "*Admin123");
            var role = await _roleManager.GetRoleNameAsync(new ContextRole(Roles.Admin));
            await _userManager.AddToRoleAsync(admin, role!);
            var claims = Policies.GetAllPolicies().Select(policy => new Claim("Permission", policy)).ToList();
            var addClaimsResult = await _userManager.AddClaimsAsync(admin, claims);
        }
    }

    private async Task AddedRolesAsync()
    {
        var rolesDoBancoDeDados = await _roleManager.Roles
                    .Select(x => x.Name)
                    .ToListAsync();

        foreach (var role in Roles.GetAllRoles())
            if (!rolesDoBancoDeDados.Contains(role))
            {
                var novaRole = new ContextRole(role)
                {
                    NormalizedName = role.ToUpper()
                };
                await _roleManager.CreateAsync(novaRole);
            }
    }

    private async Task AddedPoliciesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();

        foreach (var role in roles)
        {
            var claimsDaRole = await _roleManager.GetClaimsAsync(role);

            foreach (var policy in Policies.GetAllPolicies())
            {
                if (!claimsDaRole.Any(c => c.Type == "Permission" && c.Value == policy))
                {
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", policy));
                }
            }
        }
    }
}