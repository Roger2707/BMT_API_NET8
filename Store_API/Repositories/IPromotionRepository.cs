using Store_API.DTOs.Promotions;

namespace Store_API.Repositories
{
    public interface IPromotionRepository
    {
        public Task<List<PromotionDTO>> GetAll(string start, string end);
        public Task Create(PromotionUpsertDTO promotion);
        public Task Update(int id, PromotionUpsertDTO promotion);
        public Task Delete(int id);
        public Task<double> GetPercentageDiscount(int productId);
    }
}
