using Store_API.Data;
using StackExchange.Redis;
using Store_API.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Store_API.Services.IService;
using Store_API.Repositories.IRepositories;
using Store_API.Repositories;

namespace Store_API.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        private readonly IConnectionMultiplexer _redis;
        private readonly string _connectionString;

        public UnitOfWork(StoreContext db, IDapperService dapperService, IConfiguration config, IConnectionMultiplexer redis)
        {
            _db = db;
            _dapperService = dapperService;
            _redis = redis;
            _connectionString = config.GetConnectionString("DefaultConnection");

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
            UserAddress = new UserAddressRepository(_db, _dapperService);
            UserWarehouse = new UserWarehouseRepository(_db, _dapperService);
            Order = new OrderRepository(_dapperService, _db);
            Payment = new PaymentRepository(_db, _dapperService);
            Rating = new RatingRepository(_db, _dapperService);
            ShippingOrder = new ShippingOrderRepository(_db, _dapperService);
            StockHold = new StockHoldRepository(_db, _dapperService);
        }

        #region Repositories
        public IProductRepository Product { get; private set; }
        public IProductDetailRepository ProductDetail { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
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
        public IUserWarehouseRepository UserWarehouse { get; private set; }
        public IShippingOrderRepository ShippingOrder { get; private set; }
        public IStockHoldRepository StockHold { get; private set; }

        #endregion

        #region Transactions Handle

        public async Task BeginTransactionAsync(TransactionType type = TransactionType.Both)
        {
            if (type is TransactionType.EntityFramework or TransactionType.Both)
            {
                await _db.Database.BeginTransactionAsync();
                var tran = (SqlTransaction)_db.Database.CurrentTransaction!.GetDbTransaction();
                var conn = (SqlConnection)_db.Database.GetDbConnection();

                _dapperService.UseConnection(conn);
                _dapperService.SetTransaction(tran);
            }

            if (type is TransactionType.Dapper)
            {
                var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                var tran = conn.BeginTransaction();

                _dapperService.UseConnection(conn);
                _dapperService.SetTransaction(tran);
            }
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

        public async Task CommitAsync()
        {
            if (_db.Database.CurrentTransaction != null)
            {
                await _db.Database.CommitTransactionAsync();
            }

            await _dapperService.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            if (_db.Database.CurrentTransaction != null)
            {
                await _db.Database.RollbackTransactionAsync();
            }

            await _dapperService.RollbackTransactionAsync();
        }

        #endregion
    }
}
