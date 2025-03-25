using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using Store_API.Data;
using Store_API.Helpers;
using Store_API.IRepositories;
using Store_API.IService;
using Store_API.Models;
using Store_API.Services;
using Stripe;

namespace Store_API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapper;
        private readonly IImageService _imageService;
        private readonly EmailSenderService _emailSenderService;
        private readonly ICSVRepository _csvService;

        public UnitOfWork(StoreContext db, IDapperService dapper, IImageService imageService
            , EmailSenderService emailSenderService, ICSVRepository csvService)
        {
            _db = db;
            _dapper = dapper;
            _imageService = imageService;
            _emailSenderService = emailSenderService;
            _csvService = csvService;

            // Updated Services
            Product = new ProductRepository(_dapper, _db, _imageService, _csvService);
            Category = new CategoryRepository(_db, _dapper);
            Brand = new BrandRepository(_dapper, _db);
            Althete = new AltheteRepository(_db, _dapper, _imageService);
            Comment = new CommentRepository(_db, _dapper);
            Rating = new RatingRepository(_db, _dapper);
            Promotion = new PromotionRepository(_db, _dapper);
            UserAddress = new UserAddressRepository(_db, _dapper);
            Basket = new BasketRepository(_dapper, _db);
            Order = new OrderRepository(_dapper, _db);
            Payment = new PaymentRepository(_db);
        }

        #region Models Repository
        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public IAltheteRepository Althete { get; private set; }
        public ICommentRepository Comment { get; private set; }
        public IRatingRepository Rating { get; private set; }
        public IPromotionRepository Promotion { get; private set; }
        public IUserAddressRepository UserAddress { get; private set; }


        public IBasketRepository Basket { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IPaymentRepository Payment { get; private set; }

        #endregion

        #region EF Core Methods
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            int result = await _db.SaveChangesAsync();
            return result;
        }

        #endregion

        #region Dapper Methods
        public async Task BeginTransactionDapperAsync()
        {
            await _dapper.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _dapper.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _dapper.RollbackAsync();
        }

        public async Task CloseConnectionAsync()
        {
            await _dapper.CloseConnectionAsync();
        }

        #endregion

        #region Check Exisred Field in Table

        public async Task<bool> CheckExisted(string tableName, int id)
        {
            string query = @" SELECT COUNT(Id) as Record FROM " + tableName + " WHERE Id = @Id ";
            var p = new { id };
            dynamic result = await _dapper.QueryFirstOrDefaultAsync(query, p);
            if (CF.GetInt(result.Record) > 0) return true;
            return false;
        }
        public async Task<bool> CheckExisted(string tableName, string name)
        {
            string query = @" SELECT COUNT(Id) as Record FROM " + tableName + " WHERE Name = @Name ";
            var p = new { name = !string.IsNullOrEmpty(name) ? name.ToLower().Trim() : "" };
            dynamic result = await _dapper.QueryFirstOrDefaultAsync(query, p);
            if (CF.GetInt(result.Record) > 0) return true;
            return false;
        }

        #endregion

    }
        
}
