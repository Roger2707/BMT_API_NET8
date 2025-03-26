using Store_API.DTOs.Brands;
using Store_API.DTOs.Categories;
using Store_API.IService;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Brand>> GetAll()
        {
            var brands = await _unitOfWork.Brand.GetAllAsync();
            return brands;
        }

        public async Task<Brand> GetById(Guid brandId)
        {
            var brand = await _unitOfWork.Brand.GetByIdAsync(brandId);
            return brand;
        }
        public async Task Create(BrandDTO brandDTO)
        {
            await _unitOfWork.Brand.AddAsync
            (
                new Brand
                {
                    Id = brandDTO.Id,
                    Name = brandDTO.Name,
                    Country = brandDTO.Country
                }
            );
        }
        public async Task Update(BrandDTO brandDTO)
        {
            var existedBrand = await _unitOfWork.Brand.GetByIdAsync(brandDTO.Id);
            if (existedBrand == null) throw new Exception("Brand is not existed");

            existedBrand.Name = brandDTO.Name;
            existedBrand.Country = brandDTO.Country;

            _unitOfWork.Brand.UpdateAsync(existedBrand);
        }

        public async Task Delete(Guid brandId)
        {
            var existedBrand = await _unitOfWork.Brand.GetByIdAsync(brandId);
            if (existedBrand == null) throw new Exception("Brand is not existed");
            _unitOfWork.Brand.DeleteAsync(existedBrand);
        }
    }
}
