using Store_API.DTOs;
using Store_API.DTOs.Products;
using Store_API.Models;

namespace Store_API.Repositories
{
    public interface IProductRepository
    {
        Task<Result<int>> Create(ProductUpsertDTO productCreateDTO);
        Task<Result<int>> InsertCSV(IFormFile csvFile);
        Task<Result<int>> Update(int id, ProductUpsertDTO productUpdateDTO);
        Task<Result<int>> ChangeStatus(int id);
        Task<Result<ProductDTO>> GetById(int id);
    }
}
