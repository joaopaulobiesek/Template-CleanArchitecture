using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Template.Infra.Persistence.Contexts;

public class Dapper : IDapperConnection, IDisposable
{
    private readonly IDbConnection connection;
    private readonly Context context;

    public Dapper(IConfiguration configuration, Context context)
    {
        connection = new SqlConnection(configuration.GetConnectionString("BragaContext"));
        this.context = context;
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        => (await connection.QueryAsync<T>(sql, param, transaction)).AsList();

    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
     => await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);

    public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        => await connection.QuerySingleAsync<T>(sql, param, transaction);

    public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
     => await context.Database.ExecuteSqlInterpolatedAsync($"{sql}", cancellationToken);

    public void Dispose()
        => connection.Dispose();
}