using Store_API.DTOs.Brands;
using Store_API.DTOs.Categories;
using Store_API.Models;

namespace Store_API.IService
{
    public interface IBrandService
    {
        Task<IEnumerable<Brand>> GetAll();
        Task<Brand> GetById(Guid brandId);
        Task Create(BrandDTO brandDTO);
        Task Update(BrandDTO brandDTO);
        Task Delete(Guid brandId);
    }
}
