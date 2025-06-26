using Store_API.DTOs.Promotions;
using Store_API.Models;

namespace Store_API.Repositories.IRepositories
{
    public interface IPromotionRepository : IRepository<Promotion>
    {
        Task<IEnumerable<PromotionDTO>> GetAllPromotionsAsync();
        Task<DateTime?> GetMaxEndDateOfOnePromotionAsync(Guid categoryId, Guid brandId);
    }
}
