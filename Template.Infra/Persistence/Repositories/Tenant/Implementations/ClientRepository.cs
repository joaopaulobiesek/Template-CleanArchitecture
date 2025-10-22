using Template.Application.Common.Extensions;
using Template.Application.Common.Interfaces.IRepositories.Tenant.Implementations;
using Template.Domain;
using Template.Domain.Entity.Tenant;
using Template.Infra.Persistence.Contexts.Tenant;
using Template.Infra.Persistence.Repositories.Tenant.Base;

namespace Template.Infra.Persistence.Repositories.Tenant.Implementations
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        private readonly TenantContext _context;

        public ClientRepository(TenantContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Client> SearchIQueryable(string? src, Dictionary<string, string>? customFilter = null)
        {
            IQueryable<Client> query = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(src))
            {
                var documentOrPhoneOrZipCode = StringFormatter.RemoveNonNumericCharacters(src);

                query = query.Where(x => x.Active &&
                        x.FullName != null && x.FullName.Contains(src) ||
                        !string.IsNullOrWhiteSpace(documentOrPhoneOrZipCode) && x.DocumentNumber.Replace(".", "").Replace("/", "").Replace("-", "").Contains(documentOrPhoneOrZipCode) ||
                        !string.IsNullOrWhiteSpace(documentOrPhoneOrZipCode) && x.Phone != null && x.Phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Contains(src) ||
                        !string.IsNullOrWhiteSpace(documentOrPhoneOrZipCode) && x.ZipCode != null && x.ZipCode.Replace("-", "").Contains(src)
                );
            }

            if (customFilter != null && customFilter.Any())
            {
                var filteredQuery = query.ApplyCustomFiltersWithWhitelist(
                    customFilter,
                    "Paid"
                );

                if (filteredQuery != null)
                    query = filteredQuery;
            }

            return query;
        }

        public async Task<List<string>> GetActiveModulesAsync(string userId)
        {
            return await _context.ClientModules
                //.Where(cm => cm.ClientId == userID && cm.DeactivatedAt == null)
                .Select(cm => cm.Module)
                .ToListAsync();
        }

    }
}