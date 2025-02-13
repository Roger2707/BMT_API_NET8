using Store_API.Data;
using Store_API.DTOs;
using Store_API.Helpers;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        public BrandRepository(IDapperService dapperService, StoreContext db)
        {
            _dapperService = dapperService;
            _db = db;
        }
        public async Task Create(Brand brand)
        {
            await _db.Brands.AddAsync(brand);
        }
        public async Task<Brand> Update(int id, BrandDTO brandDTO)
        {
            Brand existedBrand = await _db.Brands.FindAsync(id);
            existedBrand.Name = brandDTO?.Name;
            existedBrand.Country = brandDTO?.Country;

            return existedBrand;
        }

        public async Task Delete(int id)
        {
            Brand existedBrand = await _db.Brands.FindAsync(id);
            _db.Brands.Remove(existedBrand);
        }

        public async Task<List<Brand>> GetAllBrands()
        {
            string query = " SELECT * FROM Brands ";
            List<dynamic> result = await _dapperService.QueryAsync(query, null);
            List<Brand> brands = new List<Brand>();
            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    Brand brand = new Brand { Id = result[i]?.Id, Name = result[i]?.Name, Country = result[i]?.Country };
                    brands.Add(brand);
                }
            }

            return brands;
        }

        public async Task<Brand> GetBrandById(int id)
        {
            string query = " SELECT * FROM Brands WHERE Id = @Id ";
            var p = new { id };

            dynamic result = await _dapperService.QueryFirstOrDefaultAsync(query, p);
            if (result != null)
            {
                return new Brand() { Id = result.Id, Name = result.Name, Country = result.Country };
            }
            return null;
        }

        public async Task<bool> CheckExisted(int id)
        {
            string query = " SELECT COUNT(1) as Record FROM Brands WHERE Id = @Id ";
            var p = new { id };

            dynamic result = await _dapperService.QueryFirstOrDefaultAsync(query, p);
            if (CF.GetInt(result.Record) > 0) return true;
            return false;
        }

        public async Task<bool> CheckExisted(string name)
        {
            string query = " SELECT COUNT(1) as Record FROM Brands WHERE Name = @Name ";
            var p = new { name };

            dynamic result = await _dapperService.QueryFirstOrDefaultAsync(query, p);
            if (CF.GetInt(result.Record) > 0) return true;
            return false;
        }
    }
}
