
using Microsoft.EntityFrameworkCore;
using Store_API.Data;

namespace Store_API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly StoreContext _db;
        private DbSet<T> _dbSet;
        public Repository(StoreContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }
        #region CRUD Operations

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Retrieve Data

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        #endregion

    }
}
