using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using Store_API.Data;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Services;
using Stripe;

namespace Store_API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _db;
        private readonly IDapperService _service;
        private readonly IImageRepository _imageService;
        private readonly EmailSenderService _emailSenderService;
        private readonly ICSVRepository _csvService;

        public UnitOfWork(StoreContext db, IDapperService service, IImageRepository imageService
            , EmailSenderService emailSenderService, ICSVRepository csvService)
        {
            _db = db;
            _service = service;
            _imageService = imageService;
            _emailSenderService = emailSenderService;
            _csvService = csvService;

            // Updated Services
            Product = new ProductRepository(_service, _db, _imageService, _csvService);
            Category = new CategoryRepository(_db, _service);
            Brand = new BrandRepository(_service, _db);
            Althete = new AltheteRepository(_db, _service, _imageService);
            Comment = new CommentRepository(_db, _service);
            Rating = new RatingRepository(_db, _service);
            Promotion = new PromotionRepository(_db, _service);
            Basket = new BasketRepository(_service, _db);
            Order = new OrderRepository(_service, _db);  
        }

        #region Models Repository
        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public IAltheteRepository Althete { get; private set; }
        public ICommentRepository Comment { get; private set; }
        public IRatingRepository Rating { get; private set; }
        public IPromotionRepository Promotion { get; private set; }


        public IBasketRepository Basket { get; private set; }
        public IOrderRepository Order { get; private set; }
        // New

        #endregion

        #region Dapper Methods
        public void BeginTrans() => _service.BeginTrans();
        public void Commit() => _service.Commit();
        public void Rollback() => _service.Rollback();
        #endregion

        #region Helpers
        public async Task<int> GetMaxId(string tableName)
        {
            string query = @" SELECT MAX(Id) as MaxId FROM " + tableName;
            List<dynamic> result = await _service.QueryAsync(query, new { });
            if (result == null) return 0;
            return CF.GetInt(result[0].MaxId);
        }
        public async Task<bool> CheckExisted(string tableName, int id)
        {
            string query = @" SELECT COUNT(Id) as Record FROM " + tableName + " WHERE Id = @Id ";
            var p = new { id };
            dynamic result = await _service.QueryFirstOrDefaultAsync(query, p);
            if (CF.GetInt(result.Record) > 0) return true;
            return false;
        }
        public async Task<bool> CheckExisted(string tableName, string name)
        {
            string query = @" SELECT COUNT(Id) as Record FROM " + tableName + " WHERE Name = @Name ";
            var p = new { name = !string.IsNullOrEmpty(name) ? name.ToLower().Trim() : "" };
            dynamic result = await _service.QueryFirstOrDefaultAsync(query, p);
            if (CF.GetInt(result.Record) > 0) return true;
            return false;
        }
        public async Task<int> SaveChanges()
        {
            int result = await _db.SaveChangesAsync();
            return result;
        }
        #endregion
    }
}
