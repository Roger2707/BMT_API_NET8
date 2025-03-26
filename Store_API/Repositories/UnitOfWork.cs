using Microsoft.EntityFrameworkCore.Storage;
using Store_API.Data;
using Store_API.Helpers;
using Store_API.IRepositories;
using Store_API.IService;

namespace Store_API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        private readonly IImageService _imageService;
        private readonly ICSVRepository _csvService;

        public UnitOfWork(StoreContext db, IDapperService dapperService, IImageService imageService
            , ICSVRepository csvService)
        {
            _db = db;
            _dapperService = dapperService;
            _imageService = imageService;
            _csvService = csvService;

            Category = new CategoryRepository(_db, _dapperService);
            Brand = new BrandRepository(_db, _dapperService);
            Promotion = new PromotionRepository(_db, _dapperService);


            Product = new ProductRepository(_dapperService, _db, _imageService, _csvService);
            Comment = new CommentRepository(_db, _dapperService);
            Rating = new RatingRepository(_db, _dapperService);
            UserAddress = new UserAddressRepository(_db, _dapperService);
            Basket = new BasketRepository(_dapperService, _db);
            Order = new OrderRepository(_dapperService, _db);
            Payment = new PaymentRepository(_db);
        }

        #region Models Repository
        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
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
