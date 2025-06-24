using Store_API.DTOs.Promotions;
using Store_API.IService;
using Store_API.Models;
using Store_API.IRepositories;

namespace Store_API.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PromotionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PromotionDTO>> GetAll()
        {
            var promotions = await _unitOfWork.Promotion.GetAllPromotionsAsync();
            return promotions.ToList();
        }

        public async Task<PromotionDTO> GetPromotion(Guid promoitonId)
        {
            var promotion = await _unitOfWork.Promotion.GetPromotionAsync(promoitonId);
            return promotion;
        }

        public async Task Create(PromotionUpsertDTO promotionUpsertDTO)
        {
            // Check end date
            var maxEndDate = await _unitOfWork.Promotion.GetMaxEndDateOfOnePromotionAsync(promotionUpsertDTO.CategoryId, promotionUpsertDTO.BrandId);
            if(maxEndDate != null)
            {
                if (promotionUpsertDTO.StartDate <= maxEndDate)
                    throw new Exception("Start Date have to greater than Max End Date of this Promotion");
            }

            var promotion = new Promotion
            {
                Id = promotionUpsertDTO.Id == Guid.Empty ? Guid.NewGuid() : promotionUpsertDTO.Id,
                BrandId = promotionUpsertDTO.BrandId,
                CategoryId = promotionUpsertDTO.CategoryId,
                StartDate = promotionUpsertDTO.StartDate,
                EndDate = promotionUpsertDTO.EndDate,
                PercentageDiscount = promotionUpsertDTO.PercentageDiscount
            };

            await _unitOfWork.Promotion.AddAsync(promotion);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(Guid promotionId)
        {
            var existedPromotion = await _unitOfWork.Promotion.GetByIdAsync(promotionId);
            if (existedPromotion == null) throw new Exception("Promotion is not exited !");
            _unitOfWork.Promotion.DeleteAsync(existedPromotion);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Update(PromotionUpsertDTO promotion)
        {
            var existedPromotion = await _unitOfWork.Promotion.GetByIdAsync(promotion.Id);
            if (existedPromotion == null) throw new Exception("Promotion is not exited !");

            existedPromotion.BrandId = promotion.BrandId;
            existedPromotion.CategoryId = promotion.CategoryId;
            existedPromotion.StartDate = promotion.StartDate;
            existedPromotion.EndDate = promotion.EndDate;
            existedPromotion.PercentageDiscount = promotion.PercentageDiscount;

            _unitOfWork.Promotion.UpdateAsync(existedPromotion);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
