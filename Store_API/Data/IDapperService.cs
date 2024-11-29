namespace Store_API.Data
{
    public interface IDapperService
    {
        public Task<List<dynamic>> QueryAsync(string query, object p);
        public Task<dynamic> QueryFirstOrDefaultAsync(string query, object p);
        public Task<int> Execute(string query, object p);
        public void CreateConnection();
        public void BeginTrans();
        public void Commit();
        public void Rollback();
        public void CloseConnection();
    }
}
