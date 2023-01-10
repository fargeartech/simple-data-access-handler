using DBHandler.DbProvider;
using DBHandler.Interfaces;
using System.Configuration;

namespace DBHandler.Implementation
{
    /// <summary>
    /// Author : Faris Jaafar
    /// Created Date : FEB 2019
    /// UPDATED : -
    /// VERSION : 2.0
    /// </summary>
    internal sealed class DBWrapperFactory
    {
        private readonly ConnectionStringSettings _connectionStringSettings;
        private readonly string _strConStrName;
        private readonly string _providerName;

        /// <summary>
        /// Faris : Directly use the ConnectionString Key Name
        /// </summary>
        /// <param name="conKeyName"></param>
        public DBWrapperFactory(string conKeyName)
        {
            _connectionStringSettings = ConfigurationManager.ConnectionStrings[conKeyName];
        }

        /// <summary>
        /// Faris : use the standard connection string name
        /// </summary>
        /// <param name="strConStrName"></param>
        /// <param name="isConStrName"></param>
        public DBWrapperFactory(string strConStrName, string providerName)
        {
            this._strConStrName = strConStrName;
            this._providerName = providerName;
        }

        public IDBWrapper CreateDatabase()
        {
            IDBWrapper database = null;
            if (_connectionStringSettings != null)
                return BuildDBProvider(ref database, _connectionStringSettings.ConnectionString);
            else
                return BuildDBProvider(ref database, _strConStrName, _providerName);
        }

        private IDBWrapper BuildDBProvider(ref IDBWrapper database, string connectionStringName)
        {
            switch (_connectionStringSettings.ProviderName.ToLowerInvariant())
            {
                case "system.data.sqlclient":
                    database = new MsSqlDataProvider(connectionStringName);
                    break;
                case "mysql.data.mysqlclient":
                    database = new MySQLDataProvider(connectionStringName);
                    break;
                case "system.data.sqlite":
                    database = new SQLLiteDataProvider(connectionStringName);
                    break;
                    //case "system.data.oleDb":
                    //    database = new OledbDataAccess(connectionStringSettings.ConnectionString);
                    //    break;
                    //case "system.data.odbc":
                    //    database = new OdbcDataAccess(connectionStringSettings.ConnectionString);
                    //    break;
            }
            return database;
        }

        private IDBWrapper BuildDBProvider(ref IDBWrapper database, string connectionStringName, string providerName)
        {
            switch (providerName.ToLowerInvariant())
            {
                case "system.data.sqlclient":
                    database = new MsSqlDataProvider(connectionStringName);
                    break;
                case "mysql.data.mysqlclient":
                    database = new MySQLDataProvider(connectionStringName);
                    break;
                case "system.data.sqlite":
                    database = new SQLLiteDataProvider(connectionStringName);
                    break;
                    //case "system.data.oleDb":
                    //    database = new OledbDataAccess(connectionStringSettings.ConnectionString);
                    //    break;
                    //case "system.data.odbc":
                    //    database = new OdbcDataAccess(connectionStringSettings.ConnectionString);
                    //    break;
            }
            return database;
        }

        public string GetProviderName()
        {
            return _connectionStringSettings.ProviderName;
        }
    }
}
