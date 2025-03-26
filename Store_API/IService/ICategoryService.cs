using Store_API.DTOs.Categories;
using Store_API.Models;

namespace Store_API.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> GetById(Guid categoryId);
        Task Create(CategoryDTO categoryDTO);
        Task Update(CategoryDTO categoryDTO);
        Task Delete(Guid categoryId);
    }
}
