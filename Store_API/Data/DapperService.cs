using Dapper;
using Microsoft.Data.SqlClient;

namespace Store_API.Data
{
    public class DapperService : IDapperService
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private readonly string _connectionString;

        public DapperService(IConfiguration _configuration)
        {
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        #region Transaction Methods

        public void UseConnection(SqlConnection connection)
        {
            _connection = connection;
        }

        public void SetTransaction(SqlTransaction transaction)
        {
            _transaction = transaction;
        }

        public SqlTransaction GetTransaction() => _transaction;
        public SqlConnection GetConnection() => _connection;

        private async Task<SqlConnection> EnsureConnectionAsync()
        {
            if (_connection != null)
                return _connection;

            var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return conn;
        }

        #endregion

        #region Methods

        public async Task<List<TResult>> QueryAsync<TResult>(string query, object p = null)
        {
            var connection = await EnsureConnectionAsync();
            return (await connection.QueryAsync<TResult>(query, p, _transaction)).ToList();
        }    

        public async Task<TResult> QueryFirstOrDefaultAsync<TResult>(string query, object p = null)
        {
            var connection = await EnsureConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<TResult>(query, p, _transaction);
        }

        public async Task<int> Execute(string query, object p = null) => await _connection.ExecuteAsync(query, p, _transaction);

        #endregion
    }
}
