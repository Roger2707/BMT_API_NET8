using System.Linq.Expressions;

namespace Store_API.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[]? includes);
        Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[]? includes);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void RemoveRangeAsync(IEnumerable<T> entities);

        // Dapper Functions
        Task<IEnumerable<TResult>> QueryAsync<TResult>(string query, object? parameters = null);
        Task<TResult> QueryFirstOrDefaultAsyncAsync<TResult>(string query, object? parameters = null);
        Task<int> ExecuteAsync(string query, object? parameters = null);
    }
}
