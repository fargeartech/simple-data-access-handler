using DBHandler.Interfaces;
using System;
using System.Data;
using System.Data.SQLite;

namespace DBHandler.DbProvider
{
    /// <summary>
    /// Author : Faris Jaafar
    /// Created Date : FEB 2019
    /// UPDATED : STUCK SQLITE DOESNT SUPPORT DATAADAPTER.. FUTURE WORKAROUND
    /// UPDATED : DEC 2020 - SOLVE (USING SYSTEM.DATA.SQLITE library instead of microsoft.data.sqlite)
    /// VERSION : 1.1
    /// </summary>
    internal sealed class SQLLiteDataProvider : IDBWrapper
    {
        private string ConnectionString { get; set; }
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private SQLiteCommand _sqlcmd;
        private bool _disposed;
        public ConnectionState CurrentConnectionState
        {
            get
            {
                if (_dbConnection == null)
                    return ConnectionState.Closed;
                return _dbConnection.State;
            }
        }

        public SQLLiteDataProvider(string connectionString)
        {
            ConnectionString = connectionString;
            _dbTransaction = null;
            _dbConnection = null;
            _sqlcmd = new SQLiteCommand();
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
                _dbConnection = new SQLiteConnection(ConnectionString);
                _dbConnection.Open();
                return _dbConnection;
            }
            return _dbConnection;
        }

        public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
        {
            _sqlcmd.CommandText = commandText;
            _sqlcmd.Connection = (SQLiteConnection)connection;
            _sqlcmd.CommandType = commandType;
            return _sqlcmd;
        }

        public IDataAdapter CreateAdapter()
        {
            return new SQLiteDataAdapter(_sqlcmd);
        }

        public IDbDataParameter CreateParameter()
        {
            return _sqlcmd.CreateParameter();
        }
        public IDbTransaction Begin()
        {
            _dbTransaction = CreateConnection().BeginTransaction();
            _sqlcmd.Transaction = (SQLiteTransaction)_dbTransaction;
            return _dbTransaction;
        }

        public void Commit()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Commit();
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
                    _sqlcmd.Dispose();

                    _dbTransaction = null;
                    _dbConnection = null;
                    _sqlcmd = null;
                }
                _disposed = true;
            }
        }
        /// <summary>
        /// faris ,destructor
        /// </summary>
        ~SQLLiteDataProvider()
        {
            dispose(false);
        }
    }
}
