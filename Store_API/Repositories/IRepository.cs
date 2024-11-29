namespace Store_API.Repositories
{
    public interface IRepository
    {
        public Task<int> GetMaxId(string tableName);
        public Task<int> SaveChanges();
    }
}
