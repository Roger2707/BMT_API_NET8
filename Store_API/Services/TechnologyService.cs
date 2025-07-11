﻿using Store_API.DTOs.Technologies;
using Store_API.Infrastructures;
using Store_API.Services.IService;

namespace Store_API.Services
{
    public class TechnologyService : ITechnologyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TechnologyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TechnologyDTO>> GetTechnologiesOfProduct(Guid productId)
        {
            var technologies = await _unitOfWork.Technology.GetTechnologies(productId);
            return technologies;
        }
    }
}
