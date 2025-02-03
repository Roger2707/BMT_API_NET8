using Microsoft.AspNetCore.Identity;
using Store_API.Data;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;
using Stripe;

namespace Store_API.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _db;
        private readonly IDapperService _service;
        private readonly IImageRepository _imageService;
        private readonly UserManager<User> _userManager;
        private readonly EmailSenderService _emailSenderService;
        public UnitOfWork(StoreContext db, IDapperService service, IImageRepository imageService
            , UserManager<User> userManager, EmailSenderService emailSenderService)
        {
            _db = db;
            _service = service;
            _imageService = imageService;
            _userManager = userManager;
            _emailSenderService = emailSenderService;

            Category = new CategoryService(_db, _service);
            Brand = new BrandService(_service, _db);
            Product = new ProductService(_db, _service, _imageService);
            Althete = new AltheteService(_db, _service, _imageService);
            Account = new AccountService(_db, _service, _imageService, _userManager, _emailSenderService);
            Basket = new BasketService(_db, _service);
            Order = new OrderService(_db, _service);
            Comment = new CommentService(_db, _service);
            Rating = new RatingService(_db, _service);
            Promotion = new PromotionService(_db, _service);
        }

        #region Models Repository
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public IProductRepository Product { get; private set; }
        public IAltheteRepository Althete { get; private set; }
        public IAccountRepository Account { get; private set; }
        public IBasketRepository Basket { get; private set; }
        public IOrderRepository Order { get; private set; }
        public ICommentRepository Comment { get; private set; }
        public IRatingRepository Rating { get; private set; }
        public IPromotionRepository Promotion { get; private set; }
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
            string query = @" SELECT COUNT(*) as Record FROM " + tableName + " WHERE Id = @Id ";
            var p = new { id = id };
            dynamic result = await _service.QueryFirstOrDefaultAsync(query, p);
            if (CF.GetInt(result.Record) > 0) return true;
            return false;
        }
        public async Task<bool> CheckExisted(string tableName, string name)
        {
            string query = @" SELECT COUNT(*) as Record FROM " + tableName + " WHERE Name = @Name ";
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
