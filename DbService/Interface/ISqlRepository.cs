using System.Data;

namespace DbService.Interface
{
    public interface ISqlRepository
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }

        Task<int> ExecuteAsync(string sql, object? param = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null);
        Task CommitAsync();
        Task RollbackAsync();
    }
}