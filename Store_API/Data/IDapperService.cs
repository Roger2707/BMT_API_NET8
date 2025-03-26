namespace Store_API.Data
{
    public interface IDapperService
    {
        public Task<List<TResult>> QueryAsync<TResult>(string query, object p);
        public Task<TResult> QueryFirstOrDefaultAsync<TResult>(string query, object p);
        public Task<int> Execute(string query, object p);

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task CloseConnectionAsync();
    }
}
