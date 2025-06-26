using Store_API.DTOs.Technologies;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Repositories.IRepositories
{
    public interface ITechnologyRepository : IRepository<Technology>
    {
        Task<IEnumerable<TechnologyDTO>> GetTechnologies(Guid productId);
    }
}
