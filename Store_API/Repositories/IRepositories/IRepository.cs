using System.Linq.Expressions;

namespace Store_API.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync<TFilter>(TFilter filter, Func<TFilter, Expression<Func<T, bool>>> filterBuilder, params Expression<Func<T, object>>[] includes);
        Task<T> FindFirstAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void RemoveRangeAsync(IEnumerable<T> entities);

        // Dapper Functions
        Task<IEnumerable<TResult>> QueryAsync<TResult>(string query, object parameters = null);
        Task<TResult> QueryFirstOrDefaultAsyncAsync<TResult>(string query, object parameters = null);
        Task<int> ExecuteAsync(string query, object parameters = null);
    }
}
