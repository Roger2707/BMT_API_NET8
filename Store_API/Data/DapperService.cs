
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        #region Open / Close Connect
        public void CreateConnection()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
            {
                if (_connection != null)
                {
                    if (_connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
                    {
                        try { _connection.Dispose(); }
                        catch { }
                        _connection = null;
                    }
                }

                if (_connection == null) _connection = new SqlConnection(_connectionString);

                _connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }

        #endregion


        #region Transaction
        public void BeginTrans()
        {
            if (_transaction == null)
            {
                CreateConnection();
                _transaction = _connection.BeginTransaction();
            }
        }

        public void Commit()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                Dispose();
            }
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_transaction != null) _transaction.Dispose();
        }

        #endregion


        #region Get / Execute Query
        public async Task<int> Execute(string query, object p)
        {
            CreateConnection();
            int result = await _connection.ExecuteAsync(query, p, _transaction);

            return result;
        }

        public async Task<List<dynamic>> QueryAsync(string query, object p)
        {
            CreateConnection();
            IEnumerable<dynamic> result = await _connection.QueryAsync(query, p, _transaction);

            return result.AsList();
        }

        public async Task<dynamic> QueryFirstOrDefaultAsync(string query, object p)
        {
            CreateConnection();
            dynamic result = await _connection.QueryFirstOrDefaultAsync(query, p, _transaction);

            return result;
        }

        #endregion
    }
}
