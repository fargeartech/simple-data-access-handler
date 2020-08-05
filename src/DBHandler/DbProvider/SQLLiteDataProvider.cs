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
    public sealed class SQLLiteDataProvider : IDBWrapper
    {
        private string ConnectionString { get; set; }
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private SQLiteCommand sqlcmd;
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
            sqlcmd = new SQLiteCommand();
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
            sqlcmd.CommandText = commandText;
            sqlcmd.Connection = (SQLiteConnection)connection;
            sqlcmd.CommandType = commandType;
            return sqlcmd;
        }

        public IDataAdapter CreateAdapter()
        {
            return new SQLiteDataAdapter(sqlcmd);
        }

        public IDbDataParameter CreateParameter()
        {
            return sqlcmd.CreateParameter();
        }
        public IDbTransaction Begin()
        {
            _dbTransaction = CreateConnection().BeginTransaction();
            sqlcmd.Transaction = (SQLiteTransaction)_dbTransaction;
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
        ~SQLLiteDataProvider()
        {
            dispose(false);
        }
    }
}
