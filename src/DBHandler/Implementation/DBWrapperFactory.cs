using DBHandler.DbProvider;
using DBHandler.Interfaces;
using System.Configuration;

namespace DBHandler.Implementation
{
    /// <summary>
    /// Author : Faris Jaafar
    /// Created Date : FEB 2019
    /// UPDATED : -
    /// VERSION : 1.0
    /// </summary>
    public sealed class DBWrapperFactory
    {
        private ConnectionStringSettings connectionStringSettings;

        public DBWrapperFactory(string connectionStringName)
        {
            connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
        }

        public IDBWrapper CreateDatabase()
        {
            IDBWrapper database = null;

            switch (connectionStringSettings.ProviderName.ToLower())
            {
                case "system.data.sqlclient":
                    database = new MsSqlDataProvider(connectionStringSettings.ConnectionString);
                    break;
                case "mysql.data.mysqlclient":
                    database = new MySQLDataProvider(connectionStringSettings.ConnectionString);
                    break;
                case "system.data.sqlite":
                    database = new SQLLiteDataProvider(connectionStringSettings.ConnectionString);
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
            return connectionStringSettings.ProviderName;
        }
    }
}
