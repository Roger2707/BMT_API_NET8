﻿using Store_API.DTOs.Categories;
using Store_API.Models;
using Store_API.Services.IService;
using Store_API.Infrastructures;

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
            var categories = await _unitOfWork.Category.FindFirstAsync(x => x.Id == categoryId);
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
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Update(CategoryDTO categoryDTO)
        {
            var existedCategory = await _unitOfWork.Category.FindFirstAsync(x => x.Id == categoryDTO.Id);
            if (existedCategory == null) throw new Exception("Category is not existed");
            existedCategory.Name = categoryDTO.Name;
            _unitOfWork.Category.UpdateAsync(existedCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(Guid categoryId)
        {
            var existedCategory = await _unitOfWork.Category.FindFirstAsync(x => x.Id == categoryId);
            if (existedCategory == null) throw new Exception("Category is not existed");
            _unitOfWork.Category.DeleteAsync(existedCategory);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
