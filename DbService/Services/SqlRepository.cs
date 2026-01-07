using Dapper;
using DbService.Interface;
using System.Data;
namespace DbService.Services;


public class SqlRepository : IDisposable, ISqlRepository
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
    private bool _disposed;

    public IDbTransaction Transaction => _transaction;

    public IDbConnection Connection => _connection;

    public SqlRepository(IDbConnection dbConnection)
    {
        _connection = dbConnection;
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        return await _connection.QueryAsync<T>(sql, param, _transaction);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
    {
        return await _connection.QueryFirstOrDefaultAsync<T>(sql, param, _transaction);
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        return await _connection.ExecuteAsync(sql, param, _transaction);
    }

    public async Task CommitAsync()
    {
        _transaction?.Commit();
        Dispose();
        await Task.CompletedTask;
    }

    public async Task RollbackAsync()
    {
        _transaction?.Rollback();
        Dispose();
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }
    }
}

