using Store_API.Data;
using Store_API.DTOs.Technologies;
using Store_API.IRepositories;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class TechnologyRepository : Repository<Technology>, ITechnologyRepository
    {
        public TechnologyRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
        }

        public async Task<IEnumerable<TechnologyDTO>> GetTechnologies(Guid productId)
        {
            string query = @"
                                SELECT 
	                                t.Name
	                                , t.Description
	                                , t.ImageUrl
                                FROM Technologies t
                                INNER JOIN ProductTechnology pt ON t.Id = pt.TechnologiesId
                                WHERE pt.ProductsId = @ProductId
                                ";

            var result = await _dapperService.QueryAsync<TechnologyDTO>(query, new { ProductId = productId });
            return result;
        }
    }
}
