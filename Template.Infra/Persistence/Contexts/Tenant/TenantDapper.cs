using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Template.Infra.Persistence.Contexts.Tenant;

public class TenantDapper : ITenantDapperConnection, IDisposable
{
    private readonly IDbConnection _connection;
    private readonly TenantContext _context;

    public TenantDapper(IConfiguration configuration, TenantContext context)
    {
        _connection = new SqlConnection(configuration.GetConnectionString(nameof(TenantContext)));
        _context = context;
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        => (await _connection.QueryAsync<T>(sql, param, transaction)).AsList();

    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
     => await _connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);

    public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        => await _connection.QuerySingleAsync<T>(sql, param, transaction);

    public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
     => await _context.Database.ExecuteSqlInterpolatedAsync($"{sql}", cancellationToken);

    public void Dispose()
        => _connection.Dispose();
}