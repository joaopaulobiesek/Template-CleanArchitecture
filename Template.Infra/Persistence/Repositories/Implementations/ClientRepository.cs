using Template.Application.Common.Interfaces.IRepositories.Implementations;
using Template.Domain;
using Template.Domain.Entity;
using Template.Infra.Persistence.Contexts;
using Template.Infra.Persistence.Repositories.Base;

namespace Template.Infra.Persistence.Repositories.Implementations
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        private readonly Context _context;

        public ClientRepository(Context context) : base(context) 
            =>_context = context;

        public IQueryable<Client> SearchIQueryable(string? src)
        {
            IQueryable<Client> query = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(src))
            {
                var documentOrPhoneOrZipCode = StringFormatter.RemoveNonNumericCharacters(src);

                query = query.Where(x => x.Active &&
                        (x.FullName != null && x.FullName.Contains(src)) ||
                        (!string.IsNullOrWhiteSpace(documentOrPhoneOrZipCode) && x.DocumentNumber.Replace(".", "").Replace("/", "").Replace("-", "").Contains(documentOrPhoneOrZipCode)) ||
                        (!string.IsNullOrWhiteSpace(documentOrPhoneOrZipCode) && x.Phone != null && x.Phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Contains(src)) ||
                        (!string.IsNullOrWhiteSpace(documentOrPhoneOrZipCode) && x.ZipCode != null && x.ZipCode.Replace("-", "").Contains(src))
                );
            }
            return query;
        }

    }
}