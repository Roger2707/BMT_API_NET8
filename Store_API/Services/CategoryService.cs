using Store_API.DTOs.Categories;
using Store_API.IService;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Category>> GetAll()
        {
            var categories = await _unitOfWork.Category.GetAllAsync();
            return categories;
        }

        public async Task<Category> GetById(Guid categoryId)
        {
            var categories = await _unitOfWork.Category.GetByIdAsync(categoryId);
            return categories;
        }

        public async Task Create(CategoryDTO categoryDTO)
        {
            await _unitOfWork.Category.AddAsync
            (
                new Category
                {
                    Id = categoryDTO.Id,
                    Name = categoryDTO.Name
                }
            );
        }

        public async Task Update(CategoryDTO categoryDTO)
        {
            var existedCategory = await _unitOfWork.Category.GetByIdAsync(categoryDTO.Id);
            if (existedCategory == null) throw new Exception("Category is not existed");
            existedCategory.Name = categoryDTO.Name;
            _unitOfWork.Category.UpdateAsync(existedCategory);
        }

        public async Task Delete(Guid categoryId)
        {
            var existedCategory = await _unitOfWork.Category.GetByIdAsync(categoryId);
            if (existedCategory == null) throw new Exception("Category is not existed");
            _unitOfWork.Category.DeleteAsync(existedCategory);
        }
    }
}
