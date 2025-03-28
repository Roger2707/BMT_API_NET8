using Microsoft.EntityFrameworkCore.Storage;
using Store_API.IRepositories;

namespace Store_API.Repositories
{
    public interface IUnitOfWork
    {
        public IProductRepository Product { get; }
        public IProductDetailRepository ProductDetail { get; }
        public ICategoryRepository Category { get; }
        public IBrandRepository Brand { get; }
        public IBasketRepository Basket { get; }
        public IOrderRepository Order { get; }
        public ICommentRepository Comment { get; }
        public IRatingRepository Rating { get; }
        public IPromotionRepository Promotion { get; }
        public IUserAddressRepository UserAddress { get; }
        public IPaymentRepository Payment { get; }
        public ITechnologyRepository Technology { get; }
        public IWarehouseRepository Warehouse { get; }
        public IStockRepository Stock { get; }
        public IStockTransactionRepository StockTransaction { get; }


        #region EF Core Methods
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();

        #endregion

        #region Dapper Methods
        public Task BeginTransactionDapperAsync();
        public Task CommitAsync();
        public Task RollbackAsync();

        public Task CloseConnectionAsync();
        #endregion

        #region Existed Field in Table
        public Task<bool> CheckExisted(string tableName, int id);
        #endregion
    }
}
