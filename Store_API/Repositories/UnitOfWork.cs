using Store_API.Data;
using Store_API.IRepositories;
using StackExchange.Redis;
using Store_API.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Store_API.IService;

namespace Store_API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        private readonly IConnectionMultiplexer _redis;

        #region Fields to manage transactions

        private readonly string _connectionString;
        private SqlConnection _sqlConnection;
        private SqlTransaction _sqlTransaction;
        private IDbContextTransaction _efTransaction;
        private TransactionType _currentType;

        #endregion

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
            UserAddress = new UserAddressRepository(_db, _dapperService);
            UserWarehouse = new UserWarehouseRepository(_db, _dapperService);
            Order = new OrderRepository(_dapperService, _db);
            Payment = new PaymentRepository(_db);
            Rating = new RatingRepository(_db, _dapperService);
            ShippingOrder = new ShippingOrderRepository(_db, _dapperService);
        }

        #region Models Repository
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

        #endregion

        #region Transaction Handle

        public async Task BeginTransactionAsync(TransactionType type)
        {
            _currentType = type;

            if (type == TransactionType.Dapper || type == TransactionType.Both)
            {
                _sqlConnection = new SqlConnection(_connectionString);
                await _sqlConnection.OpenAsync();
                _sqlTransaction = _sqlConnection.BeginTransaction();

                _dapperService.UseConnection(_sqlConnection);
                _dapperService.SetTransaction(_sqlTransaction);
            }

            if (type == TransactionType.EntityFramework || type == TransactionType.Both)
            {
                _efTransaction = await _db.Database.BeginTransactionAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync(); 
        }

        public async Task CommitAsync()
        {
            if (_currentType == TransactionType.EntityFramework || _currentType == TransactionType.Both)
            {
                await _efTransaction?.CommitAsync();
            }
            if (_currentType == TransactionType.Dapper || _currentType == TransactionType.Both)
            {
                _sqlTransaction?.Commit();
                await _sqlConnection?.CloseAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_currentType == TransactionType.EntityFramework || _currentType == TransactionType.Both)
                await _efTransaction?.RollbackAsync();

            if (_currentType == TransactionType.Dapper || _currentType == TransactionType.Both)
            {
                _sqlTransaction?.Rollback();
                await _sqlConnection?.CloseAsync();
            }
        }

        #endregion
    }
}
