using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace DBHandler.DbProvider
{
    /// <summary>
    /// Author : Faris Jaafar
    /// Created Date : FEB 2019
    /// UPDATED : -
    /// VERSION : 1.0
    /// </summary>
    public sealed class ParameterManager
    {
        public static IDbDataParameter CreateParameter(string providerName, string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            IDbDataParameter parameter = null;
            switch (providerName.ToLower())
            {
                case "system.data.sqlclient":
                    return CreateSqlParameter(name, value, dbType, direction);
                case "mysql.data.mysqlclient":
                    return CreateMySqlParameter(name, value, dbType, direction);
                case "system.data.sqlite":
                    return CreateSQLiteParameter(name, value, dbType, direction);
                case "system.data.oleDb":
                    break;
                case "system.data.odbc":
                    break;
            }

            return parameter;
        }

        public static IDbDataParameter CreateParameter(string providerName, string name, int size, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            IDbDataParameter parameter = null;
            switch (providerName.ToLower())
            {
                case "system.data.sqlclient":
                    return CreateSqlParameter(name, size, value, dbType, direction);
                case "mysql.data.mysqlclient":
                    return CreateMySqlParameter(name, size, value, dbType, direction);
                case "system.data.sqlite":
                    return CreateSQLiteParameter(name, size, value, dbType, direction);
                case "system.data.oleDb":
                    break;
                case "system.data.odbc":
                    break;
            }

            return parameter;
        }

        private static IDbDataParameter CreateSqlParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            return new SqlParameter
            {
                DbType = dbType,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static IDbDataParameter CreateSqlParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            return new SqlParameter
            {
                DbType = dbType,
                Size = size,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static IDbDataParameter CreateMySqlParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            return new MySqlParameter
            {
                DbType = dbType,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static IDbDataParameter CreateMySqlParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            return new MySqlParameter
            {
                DbType = dbType,
                Size = size,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static IDbDataParameter CreateSQLiteParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            return new SQLiteParameter
            {
                DbType = dbType,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static IDbDataParameter CreateSQLiteParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            return new SQLiteParameter
            {
                DbType = dbType,
                Size = size,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }
        //private static IDbDataParameter CreateOracleParameter(string name, object value, DbType dbType, ParameterDirection direction)
        //{
        //    return new OracleParameter
        //    {
        //        DbType = dbType,
        //        ParameterName = name,
        //        Direction = direction,
        //        Value = value
        //    };
        //}

        //private static IDbDataParameter CreateOracleParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        //{
        //    return new OracleParameter
        //    {
        //        DbType = dbType,
        //        Size = size,
        //        ParameterName = name,
        //        Direction = direction,
        //        Value = value
        //    };
        //}
    }
}
