using Store_API.DTOs;
using Store_API.Models;

namespace Store_API.Repositories
{
    public interface IBrandRepository
    {
        public Task<List<Brand>> GetAllBrands();
        public Task<Brand> GetBrandById(int id);

        public Task Create(Brand brand);
        public Task<Brand> Update(int id, BrandDTO brandDTO);
        public Task Delete(int id);

        public Task<bool> CheckExisted(int id);
        public Task<bool> CheckExisted(string name);
    }
}
