using Dapper;
using Microsoft.Data.SqlClient;
using Store_API.Services.IService;

namespace Store_API.Services
{
    public class DapperService : IDapperService
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        #region Transactions

        public void UseConnection(SqlConnection connection) => _connection = connection;

        public void SetTransaction(SqlTransaction transaction) => _transaction = transaction;

        private SqlConnection EnsureConnectionAsync()
        {
            if (_connection != null) return _connection;
            throw new InvalidOperationException("DapperService is not initialized with a connection.");
        }

        public async Task CommitTransactionAsync()
        {
            _transaction?.Commit();
            if (_connection != null) await _connection.CloseAsync();
            _transaction = null;
            _connection = null;
        }

        public async Task RollbackTransactionAsync()
        {
            _transaction?.Rollback();
            if (_connection != null) await _connection.CloseAsync();
            _transaction = null;
            _connection = null;
        }

        #endregion

        #region CRUD

        public async Task<List<TResult>> QueryAsync<TResult>(string query, object p = null)
        {
            var connection = EnsureConnectionAsync();
            return (await connection.QueryAsync<TResult>(query, p, _transaction)).ToList();
        }

        public async Task<TResult> QueryFirstOrDefaultAsync<TResult>(string query, object p = null)
        {
            var connection = EnsureConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<TResult>(query, p, _transaction);
        }

        public async Task<int> Execute(string query, object p = null)
        {
            var connection = EnsureConnectionAsync();
            return await connection.ExecuteAsync(query, p, _transaction);
        }

        #endregion
    }
}
