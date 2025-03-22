using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Store_API.Data
{
    public class DapperService : IDapperService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public DapperService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        #region Transaction Methods
        public async Task BeginTransactionAsync()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                _connection = new SqlConnection(_connectionString);
                await _connection.OpenAsync();
            }
            _transaction = (SqlTransaction)await _connection.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                DisposeTransaction();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                DisposeTransaction();
            }
        }

        public async Task CloseConnectionAsync()
        {
            if (_connection != null && _transaction == null && _connection.State != ConnectionState.Closed)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
                _connection = null;
            }
        }

        private void DisposeTransaction()
        {
            _transaction?.Dispose();
            _transaction = null;
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }

        public void Dispose()
        {
            DisposeTransaction();
        }

        #endregion

        #region Queries Methods
        public async Task<int> Execute(string query, object p = null)
        {
            if (_transaction != null)
            {
                // Sử dụng giao dịch nếu đã bắt đầu giao dịch trước đó
                return await _connection.ExecuteAsync(query, p, _transaction);
            }
            else
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();  // Mở kết nối bất đồng bộ
                    return await connection.ExecuteAsync(query, p);
                }
            }
        }

        public async Task<List<dynamic>> QueryAsync(string query, object p)
        {
            if (_transaction != null)
            {
                // Sử dụng giao dịch nếu đã bắt đầu giao dịch trước đó
                return (await _connection.QueryAsync<dynamic>(query, p, _transaction)).ToList();
            }
            else
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();  // Mở kết nối bất đồng bộ
                    return (await connection.QueryAsync<dynamic>(query, p)).ToList();
                }
            }
        }

        public async Task<dynamic> QueryFirstOrDefaultAsync(string query, object p)
        {
            if (_transaction != null)
            {
                // Sử dụng giao dịch nếu đã bắt đầu giao dịch trước đó
                return await _connection.QueryFirstOrDefaultAsync<dynamic>(query, p, _transaction);
            }
            else
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();  // Mở kết nối bất đồng bộ
                    return await connection.QueryFirstOrDefaultAsync<dynamic>(query, p);
                }
            }
        }

        #endregion
    }
}
