using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class CategoryService : ICategoryRepository
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;

        public CategoryService(StoreContext db, IDapperService dapperService)
        {
            _db = db;
            _dapperService = dapperService;
        }

        public async Task Create(Category category)
        {
            await _db.Categories.AddAsync(category);
        }
        public async Task<Category> Update(int id, string name)
        {
            Category category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if(!string.IsNullOrEmpty(name)) 
                category.Name = name;

            return category;
        }

        public async Task Delete(int categoryId)
        {
            Category category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            _db.Categories.Remove(category);
        }

        public async Task<List<Category>> GetAllCategory()
        {
            string query = " SELECT * FROM Categories ";
            List<Category> categories = new List<Category>();
            List<dynamic> result = await _dapperService.QueryAsync(query, null);

            for (int i = 0; i < result.Count; i++)
            {
                Category category = new() { Id = result[i].Id, Name = result[i].Name };
                categories.Add(category);
            }

            return categories;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            string query = " SELECT * FROM Categories WHERE Id = @Id";
            var p = new { id = id };

            dynamic result = await _dapperService.QueryFirstOrDefaultAsync(query, p);
            Category category = new Category { Id = result.Id, Name = result.Name };
            return category;
        }

        public async Task<bool> CheckCategoryExisted(string name)
        {
            string query = @" SELECT COUNT(*) as Record FROM Categories WHERE Name = @Name ";
            var p = new { name = name };

            var result = await _dapperService.QueryFirstOrDefaultAsync(query, p);

            if (CF.GetInt(result.Record) > 0) return true;
            return false;
        }
    }
}
