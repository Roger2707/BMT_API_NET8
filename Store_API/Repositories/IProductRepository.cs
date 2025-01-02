using Store_API.DTOs;
using Store_API.DTOs.Products;
using Store_API.Models;

namespace Store_API.Repositories
{
    public interface IProductRepository
    {
        public Task<ProductDTO> GetById(int id);
        public Task<int> GetTotalRecord(ProductParams productParams);
        public Task<List<ProductDTO>> GetSourceProducts(ProductParams productParams);
        public Task<Pagination<ProductDTO>> GetPagination(List<ProductDTO> products, ProductParams productParams);
        public Task<dynamic> GetTechnologies(int productId);
        public Task<int> Create(ProductUpsertDTO productCreateDTO);
        public Task<int> InsertCSV(ProductCSV productCSV);
        public Task<int> Update(int id, ProductUpsertDTO productCreateDTO);
        public Task<int> ChangeStatus(int id);
    }
}
