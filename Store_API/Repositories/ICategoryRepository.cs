using Store_API.Models;

namespace Store_API.Repositories
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetAllCategory();
        public Task<Category> GetCategoryById(int id);
        public Task Create(Category category);
        public Task<Category> Update(int id, string name);
        public Task Delete(int categoryId);

        public Task<bool> CheckCategoryExisted(string name);
    }
}
