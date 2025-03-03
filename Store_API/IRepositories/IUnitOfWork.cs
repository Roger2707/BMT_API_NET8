using Store_API.IRepositories;

namespace Store_API.Repositories
{
    public interface IUnitOfWork
    {
        public IProductRepository Product { get; }

        public ICategoryRepository Category { get; }
        public IBrandRepository Brand { get; }
        public IAltheteRepository Althete { get; }
        public IBasketRepository Basket { get; }
        public IOrderRepository Order { get; }
        public ICommentRepository Comment { get; }
        public IRatingRepository Rating { get; }
        public IPromotionRepository Promotion { get; }
        public IUserAddressRepository UserAddress { get; }

        Task<int> SaveChanges();

        public void BeginTrans();
        public void Commit();
        public void Rollback();
        public Task<int> GetMaxId(string tableName);
        public Task<bool> CheckExisted(string tableName, int id);
        public Task<bool> CheckExisted(string tableName, string name);
    }
}
