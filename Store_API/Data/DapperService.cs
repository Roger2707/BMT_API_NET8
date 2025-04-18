using Dapper;
using Microsoft.Data.SqlClient;

namespace Store_API.Data
{
    public class DapperService : IDapperService
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

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

        #endregion

        #region Methods

        public async Task<List<TResult>> QueryAsync<TResult>(string query, object p = null)
            => (await _connection.QueryAsync<TResult>(query, p)).ToList();

        public async Task<TResult> QueryFirstOrDefaultAsync<TResult>(string query, object p = null)
            => await _connection.QueryFirstOrDefaultAsync<TResult>(query, p);

        public async Task<int> Execute(string query, object p = null)
            => await _connection.ExecuteAsync(query, p, _transaction);

        #endregion
    }
}
