using Template.Application.Common.Interfaces.IRepositories.Base;
using Template.Domain.Entity;

namespace Template.Application.Common.Interfaces.IRepositories.Implementations;

public interface IClientRepository : IRepository<Client>
{
    IQueryable<Client> SearchIQueryable(string? src);
}