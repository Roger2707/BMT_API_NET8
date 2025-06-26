using Dapper;
using Microsoft.Data.SqlClient;
using Store_API.Services.IService;
using System.Data;

namespace Store_API.Services
{
    public class DapperService : IDapperService
    {
        private readonly string _connectionString;
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public DapperService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        #region Transactions

        public void UseConnection(SqlConnection connection) => _connection = connection;

        public void SetTransaction(SqlTransaction transaction) => _transaction = transaction;

        private async Task<SqlConnection> EnsureConnectionAsync()
        {
            if (_connection != null) return _connection;
            //throw new InvalidOperationException("DapperService is not initialized with a connection.");

            var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return conn;
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null || _transaction.Connection == null)
            {
                _transaction = null;
                _connection = null;
                return;
            }

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;

            if (_connection.State == ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }

            _connection = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null || _transaction.Connection == null)
            {
                _transaction = null;
                _connection = null;
                return;
            }

            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;

            if (_connection.State == ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }

            _connection = null;
        }

        #endregion

        #region CRUD

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

        public async Task<int> Execute(string query, object p = null)
        {
            var connection = await EnsureConnectionAsync();
            return await connection.ExecuteAsync(query, p, _transaction);
        }

        #endregion
    }
}
