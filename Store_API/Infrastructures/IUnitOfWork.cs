using Store_API.Enums;
using Store_API.Repositories.IRepositories;

namespace Store_API.Infrastructures
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync(TransactionType type = TransactionType.Both);
        Task SaveChangesAsync();
        Task CommitAsync();
        Task RollbackAsync();

        #region Repositories

        public IUserRepository User { get; }
        public IProductRepository Product { get; }
        public IProductDetailRepository ProductDetail { get; }
        public ICategoryRepository Category { get; }
        public IBrandRepository Brand { get; }
        public IBasketRepository Basket { get; }
        public IOrderRepository Order { get; }
        public IRatingRepository Rating { get; }
        public IPromotionRepository Promotion { get; }
        public IUserAddressRepository UserAddress { get; }
        public IPaymentRepository Payment { get; }
        public ITechnologyRepository Technology { get; }
        public IWarehouseRepository Warehouse { get; }
        public IStockRepository Stock { get; }
        public IStockTransactionRepository StockTransaction { get; }
        public IUserWarehouseRepository UserWarehouse { get; }
        public IShippingOrderRepository ShippingOrder { get; }
        public IStockHoldRepository StockHold { get; }

        #endregion
    }
}
