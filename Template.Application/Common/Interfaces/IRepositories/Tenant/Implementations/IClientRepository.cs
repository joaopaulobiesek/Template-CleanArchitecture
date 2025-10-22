using Template.Application.Common.Interfaces.IRepositories.Tenant.Base;
using Template.Domain.Entity.Tenant;

namespace Template.Application.Common.Interfaces.IRepositories.Tenant.Implementations;

public interface IClientRepository : IRepository<Client>
{
    IQueryable<Client> SearchIQueryable(string? src, Dictionary<string, string>? customFilter = null);
    Task<List<string>> GetActiveModulesAsync(string userId);
}