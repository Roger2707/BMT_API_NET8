using Store_API.DTOs.Technologies;

namespace Store_API.Services.IService
{
    public interface ITechnologyService
    {
        Task<IEnumerable<TechnologyDTO>> GetTechnologiesOfProduct(Guid productId);
    }
}
