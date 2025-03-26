using Store_API.DTOs.Promotions;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Models;
using Store_API.Repositories;

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
            if (promotionUpsertDTO.Start.Length != 8 || promotionUpsertDTO.End.Length != 8)
                throw new Exception("Date is invalid");

            DateTime start = new DateTime(CF.GetInt(promotionUpsertDTO.Start.Substring(0, 4)), CF.GetInt(promotionUpsertDTO.Start.Substring(4, 2)), CF.GetInt(promotionUpsertDTO.Start.Substring(6, 2)));
            DateTime end = new DateTime(CF.GetInt(promotionUpsertDTO.End.Substring(0, 4)), CF.GetInt(promotionUpsertDTO.End.Substring(4, 2)), CF.GetInt(promotionUpsertDTO.End.Substring(6, 2)));

            if (start < DateTime.Now)
                throw new Exception("Start Date have to greater than or equal Current Date");

            if (start >= end)
                throw new Exception("Start Date have to smaller than End Date");

            // Check end date
            var maxEndDate = await _unitOfWork.Promotion.GetMaxEndDateOfOnePromotionAsync(promotionUpsertDTO.CategoryId, promotionUpsertDTO.BrandId);
            if(maxEndDate != null)
            {
                if (start <= maxEndDate)
                    throw new Exception("Start Date have to greater than Max End Date of this Promotion");
            }

            var promotion = new Promotion
            {
                Id = promotionUpsertDTO.Id,
                BrandId = promotionUpsertDTO.BrandId,
                CategoryId = promotionUpsertDTO.CategoryId,
                Start = start,
                End = end,
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
            existedPromotion.Start = new DateTime(CF.GetInt(promotion.Start.Substring(0, 4)), CF.GetInt(promotion.Start.Substring(4, 2)), CF.GetInt(promotion.Start.Substring(6, 2)));
            existedPromotion.End = new DateTime(CF.GetInt(promotion.End.Substring(0, 4)), CF.GetInt(promotion.End.Substring(4, 2)), CF.GetInt(promotion.End.Substring(6, 2)));
            existedPromotion.PercentageDiscount = promotion.PercentageDiscount;

            _unitOfWork.Promotion.UpdateAsync(existedPromotion);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
