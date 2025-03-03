using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EventSourcing.Projections.Database
{
    public class ReadStoreConnection(IConfiguration configuration) : IDisposable
    {
        private readonly string? _readStoreConnectionString
       = configuration.GetConnectionString("ReadStore");

        private SqlConnection? _connection;
        private SqlTransaction? _transaction;

        public IDbConnection GetConnection()
        {
            _connection ??= new SqlConnection(_readStoreConnectionString);

            return _connection;
        }

        public IDbTransaction GetTransaction()
        {
            if (_transaction is not null)
                return _transaction;

            _connection ??= new SqlConnection(_readStoreConnectionString);

            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            _transaction = _connection.BeginTransaction();
            return _transaction;
        }

        void IDisposable.Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
