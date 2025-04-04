using Microsoft.EntityFrameworkCore.Storage;
using Store_API.Data;
using Store_API.Helpers;
using Store_API.IRepositories;
using StackExchange.Redis;

namespace Store_API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        private readonly IConnectionMultiplexer _redis;

        public UnitOfWork(StoreContext db, IDapperService dapperService, IConnectionMultiplexer redis)
        {
            _db = db;
            _dapperService = dapperService;
            _redis = redis;

            Category = new CategoryRepository(_db, _dapperService);
            Brand = new BrandRepository(_db, _dapperService);
            Promotion = new PromotionRepository(_db, _dapperService);
            Product = new ProductRepository(_db, _dapperService);
            ProductDetail = new ProductDetailRepository(_db, _dapperService);
            Technology = new TechnologyRepository(_db, _dapperService);
            Warehouse = new WarehouseRepository(_db, _dapperService);
            Stock = new StockRepository(_db, _dapperService);
            StockTransaction = new StockTransactionRepository(_db, _dapperService);
            Basket = new BasketRepository(_dapperService, _redis);

            Comment = new CommentRepository(_db, _dapperService);
            Rating = new RatingRepository(_db, _dapperService);
            UserAddress = new UserAddressRepository(_db, _dapperService);
            Order = new OrderRepository(_dapperService, _db);
            Payment = new PaymentRepository(_db);
        }

        #region Models Repository
        public IProductRepository Product { get; private set; }
        public IProductDetailRepository ProductDetail { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public ICommentRepository Comment { get; private set; }
        public IRatingRepository Rating { get; private set; }
        public IPromotionRepository Promotion { get; private set; }
        public IUserAddressRepository UserAddress { get; private set; }
        public IBasketRepository Basket { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IPaymentRepository Payment { get; private set; }
        public ITechnologyRepository Technology { get; private set; }
        public IWarehouseRepository Warehouse { get; private set; }
        public IStockRepository Stock { get; private set; }
        public IStockTransactionRepository StockTransaction { get; private set; }

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
            await _dapperService.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _dapperService.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _dapperService.RollbackAsync();
        }

        public async Task CloseConnectionAsync()
        {
            await _dapperService.CloseConnectionAsync();
        }

        #endregion

        #region Check Exisred Field in Table

        public async Task<bool> CheckExisted(string tableName, int id)
        {
            string query = @" SELECT COUNT(Id) as Record FROM " + tableName + " WHERE Id = @Id ";
            var p = new { id };
            int result = await _dapperService.QueryFirstOrDefaultAsync<int>(query, p);
            if (CF.GetInt(result) > 0) return true;
            return false;
        }

        #endregion

    }
        
}
