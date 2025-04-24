using Microsoft.Data.SqlClient;

namespace Store_API.IService
{
    public interface IDapperService
    {
        void UseConnection(SqlConnection connection);
        void SetTransaction(SqlTransaction transaction);
        SqlTransaction GetTransaction();
        SqlConnection GetConnection();

        public Task<List<TResult>> QueryAsync<TResult>(string query, object p = null);
        public Task<TResult> QueryFirstOrDefaultAsync<TResult>(string query, object p = null);
        public Task<int> Execute(string query, object p = null);
    }
}
