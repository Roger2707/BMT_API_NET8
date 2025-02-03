
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Store_API.Data
{
    public class DapperService : IDapperService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private IDbConnection _connection = null;
        private IDbTransaction _transaction = null;

        public DapperService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        #region Transaction Methods
        public void BeginTrans()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _transaction?.Commit();
            }
            catch
            {
                Rollback();  // Nếu commit thất bại, rollback
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        public void Rollback()
        {
            try
            {
                _transaction?.Rollback();
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        // Đảm bảo đóng kết nối khi giao dịch kết thúc hoặc khi không còn cần thiết
        private void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
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
