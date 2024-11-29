namespace Store_API.Repositories
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Category { get; }
        public IBrandRepository Brand { get; }
        public IProductRepository Product { get; }
        public IAltheteRepository Althete { get; }
        public IAccountRepository Account { get; }
        public IBasketRepository Basket { get; }
        public IOrderRepository Order { get; }
        public ICommentRepository Comment { get; }
        public IRatingRepository Rating { get; }
        public IPromotionRepository Promotion { get; }

        Task<int> SaveChanges();

        public void BeginTrans();
        public void Commit();
        public void Rollback();
        public void CloseConnection();
        public Task<int> GetMaxId(string tableName);
        public Task<bool> CheckExisted(string tableName, int id);
        public Task<bool> CheckExisted(string tableName, string name);
    }
}
