using Store_API.Data;
using Store_API.Helpers;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class RepositoryService : IRepository
    {
        private readonly IDapperService _dapperService;
        private readonly StoreContext _db;
        public RepositoryService(IDapperService dapperService, StoreContext db)
        {
            _dapperService = dapperService;
            _db = db;
        }
        public async Task<int> GetMaxId(string tableName)
        {
            string query = @" SELECT MAX(Id) as MaxId FROM " + tableName;
            List<dynamic> result = await _dapperService.QueryAsync(query, new { });
            if (result == null) return 0;
            return CF.GetInt(result[0].MaxId);
        }

        public async Task<int> SaveChanges()
        {
            int result = await _db.SaveChangesAsync();
            return result;
        }
    }
}
