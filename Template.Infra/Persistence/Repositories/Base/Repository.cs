using Template.Application.Common.Interfaces.IRepositories.Base;
using Template.Infra.Persistence.Contexts;

namespace Template.Infra.Persistence.Repositories.Base;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly Context _context;

    public Repository(Context context)
    {
        _context = context;
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        return entity ?? throw new KeyNotFoundException($"Entity with ID '{id}' not found.");
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        => await _context.Set<T>().ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
        => await _context.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity)
        => _context.Set<T>().Update(entity);

    public void Delete(T entity)
        => _context.Set<T>().Remove(entity);
}