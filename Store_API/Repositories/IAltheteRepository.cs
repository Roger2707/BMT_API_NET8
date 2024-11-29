using Store_API.DTOs.Althetes;
using Store_API.Models;

namespace Store_API.Repositories
{
    public interface IAltheteRepository
    {
        public Task<List<Althete>> GetAll();
        public Task<AltheteDetailDTO> GetPlayerById(int id);
        public Task Create(AltheteUpsertDTO playerDTO);
        public Task<Althete> Update(int id, AltheteUpsertDTO playerDTO);
        public Task Delete(int id);
    }
}
