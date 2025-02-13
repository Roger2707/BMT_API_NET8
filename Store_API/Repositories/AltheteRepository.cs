using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Althetes;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class AltheteRepository : IAltheteRepository
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        private readonly IImageRepository _imageService;

        public AltheteRepository(StoreContext db, IDapperService dapperService, IImageRepository imageService)
        {
            _db = db;
            _dapperService = dapperService;
            _imageService = imageService;
        }

        public async Task Create(AltheteUpsertDTO playerDTO)
        {
            var player = new Althete()
            {
                Name = playerDTO?.Name,
                Country = playerDTO?.Country,
                Achivement = playerDTO?.Achivement,
                Info = playerDTO?.Info,
                ProductId = playerDTO.ProductId,
            };

            if (playerDTO.PictureUrl != null)
            {
                var imageResult = await _imageService.AddImageAsync(playerDTO.PictureUrl, "althetes");

                if (imageResult.Error != null) throw new Exception(imageResult.Error.Message);

                player.PictureUrl = imageResult.SecureUrl.ToString();
                player.PublicId = imageResult.PublicId;
            }

            await _db.Althetes.AddAsync(player);
        }
        public async Task<Althete> Update(int id, AltheteUpsertDTO playerDTO)
        {
            Althete existedPlayer = await _db.Althetes.FindAsync(id);

            if (playerDTO.Name != "" && playerDTO.Name != existedPlayer.Name)
                existedPlayer.Name = playerDTO.Name;

            if (playerDTO.Country != "" && playerDTO.Country != existedPlayer.Country)
                existedPlayer.Country = playerDTO.Country;

            if (playerDTO.Achivement != "" && playerDTO.Achivement != existedPlayer.Achivement)
                existedPlayer.Achivement = playerDTO.Achivement;

            if (playerDTO.Info != "" && playerDTO.Info != existedPlayer.Info)
                existedPlayer.Info = playerDTO.Info;

            if (playerDTO.ProductId != playerDTO.ProductId)
                existedPlayer.ProductId = playerDTO.ProductId;

            if (playerDTO.PictureUrl != null)
            {
                var imageUploadResult = await _imageService.AddImageAsync(playerDTO.PictureUrl, "althetes");

                if (imageUploadResult.Error != null)
                    throw new Exception(imageUploadResult.Error.Message);

                if (!string.IsNullOrEmpty(existedPlayer.PublicId))
                    await _imageService.DeleteImageAsync(existedPlayer.PublicId);

                existedPlayer.PictureUrl = imageUploadResult.SecureUrl.ToString();
                existedPlayer.PublicId = imageUploadResult.PublicId;
            }

            return existedPlayer;
        }

        public async Task Delete(int id)
        {
            var existedPlayer = await _db.Althetes.FindAsync(id);
            _db.Althetes.Remove(existedPlayer);
        }

        public async Task<List<Althete>> GetAll()
        {
            var players = await _db.Althetes.ToListAsync();
            return players;
        }

        public async Task<AltheteDetailDTO> GetPlayerById(int id)
        {
            string query = @"   SELECT * 
                                FROM Althetes as althete
                                INNER JOIN Products product ON althete.ProductId = product.Id
                                WHERE althete.Id = @Id ";
            var p = new { Id = id };
            dynamic result = await _dapperService.QueryFirstOrDefaultAsync(query, p);
            if (result == null) return null;

            var althete = new AltheteDetailDTO
            {
                Id = result?.Id,
                Name = result?.Name,
                Country = result?.Country,
                Achivement = result?.Achivement,
                Info = result?.Info,
                PictureUrl = result?.PictureUrl,
                PublicId = result?.PublicId,
                ProductId = result?.ProductId,
                ProductName = result?.ProductName
            };

            return althete;
        }


    }
}
