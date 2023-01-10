using DBHandler.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace DBHandler.DbProvider
{
    /// <summary>
    /// Author : Faris Jaafar
    /// Created Date : FEB 2019
    /// UPDATE : -
    /// VERSION : 1.0
    /// </summary>
    internal sealed class MySQLDataProvider : IDBWrapper
    {
        private string ConnectionString { get; set; }
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private MySqlCommand sqlcmd;
        private bool _disposed;
        public ConnectionState CurrentConnectionState
        {
            get
            {
                return _dbConnection.State;
            }
        }

        public MySQLDataProvider(string connectionString)
        {
            ConnectionString = connectionString;
            _dbTransaction = null;
            _dbConnection = null;
            sqlcmd = new MySqlCommand();
        }
        public void CloseConnection()
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
        }
        public IDbConnection CreateConnection()
        {
            //check db persistent ...
            if (_dbConnection == null && (CurrentConnectionState == ConnectionState.Broken
                || CurrentConnectionState == ConnectionState.Closed))
            {
                _dbConnection = new MySqlConnection(ConnectionString);
                _dbConnection.Open();
                return _dbConnection;
            }
            return _dbConnection;
        }

        public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
        {
            sqlcmd.CommandText = commandText;
            sqlcmd.Connection = (MySqlConnection)connection;
            sqlcmd.CommandType = commandType;
            return sqlcmd;
        }

        public IDataAdapter CreateAdapter()
        {
            return new MySqlDataAdapter(sqlcmd);
        }

        public IDbDataParameter CreateParameter()
        {
            return sqlcmd.CreateParameter();
        }
        public IDbTransaction Begin()
        {
            _dbTransaction = CreateConnection().BeginTransaction();
            sqlcmd.Transaction = (MySqlTransaction)_dbTransaction;
            return _dbTransaction;
        }

        public void Commit()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Commit();
                sqlcmd.Parameters.Clear();
                _dbTransaction.Dispose();
            }
            else
                throw new ApplicationException("Transaction is not even open");
        }

        public void Rollback()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Rollback();
                sqlcmd.Parameters.Clear();
                _dbTransaction.Dispose();
            }
            else
                throw new ApplicationException("Transaction is not even open");
        }
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }
        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_dbConnection != null && CurrentConnectionState == ConnectionState.Open)
                        CloseConnection();
                    sqlcmd.Dispose();

                    _dbTransaction = null;
                    _dbConnection = null;
                    sqlcmd = null;
                }
                _disposed = true;
            }
        }
        /// <summary>
        /// faris ,destructor
        /// </summary>
        ~MySQLDataProvider()
        {
            dispose(false);
        }
    }
}
