using Store_API.DTOs.Promotions;
using Store_API.Models;

namespace Store_API.IRepositories
{
    public interface IPromotionRepository : IRepository<Promotion>
    {
        Task<IEnumerable<PromotionDTO>> GetAllPromotionsAsync();
        Task<PromotionDTO> GetPromotionAsync(Guid promotionId);
        Task<DateTime?> GetMaxEndDateOfOnePromotionAsync(Guid categoryId, Guid brandId);
    }
}
