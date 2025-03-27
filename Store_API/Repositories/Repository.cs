
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using System.Linq.Expressions;

namespace Store_API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly StoreContext _db;
        protected readonly IDapperService _dapperService;

        private DbSet<T> _dbSet;
        public Repository(StoreContext db, IDapperService dapperService)
        {
            _db = db;
            _dbSet = db.Set<T>();
            _dapperService = dapperService;
        }

        #region CRUD Operations

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public void DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void RemoveRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        #endregion

        #region Retrieve Data

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null) query = query.Where(filter);
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[]? includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        #endregion

        #region Dapper Funcitonalities
        public async Task<IEnumerable<TResult>> QueryAsync<TResult>(string query, object parameters = null)
        {
            var result = await _dapperService.QueryAsync<TResult>(query, parameters);
            return result;
        }

        public async Task<TResult> QueryFirstOrDefaultAsyncAsync<TResult>(string query, object parameters = null)
        {
            var result = await _dapperService.QueryFirstOrDefaultAsync<TResult>(query, parameters);
            return result;
        }

        public async Task<int> ExecuteAsync(string query, object parameters = null)
        {
            return await _dapperService.Execute(query, parameters);
        }

        #endregion
    }
}
