using Store_API.DTOs.Promotions;

namespace Store_API.IService
{
    public interface IPromotionService
    {
        Task<List<PromotionDTO>> GetAll();
        Task<PromotionDTO> GetPromotion(Guid promotionId);
        Task Create(PromotionUpsertDTO promotion);
        Task Update(PromotionUpsertDTO promotion);
        Task Delete(Guid promotionId);
    }
}
