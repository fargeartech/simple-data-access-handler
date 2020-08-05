using System;
using System.Data;

namespace DBHandler.Interfaces
{
    /// <summary>
    /// Author : Faris Jaafar
    /// Created Date : FEB 2019
    /// UPDATED : -
    /// VERSION : 1.0
    /// </summary>
    public interface IDBWrapper : IDisposable
    {
        IDbConnection CreateConnection();
        void CloseConnection();
        IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection);
        IDataAdapter CreateAdapter();
        IDbDataParameter CreateParameter();
        ConnectionState CurrentConnectionState { get; }
        IDbTransaction Begin();
        void Commit();
        void Rollback();
    }
}
