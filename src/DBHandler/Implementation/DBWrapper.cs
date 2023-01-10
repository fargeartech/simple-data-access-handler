using DBHandler.DbProvider;
using DBHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DBHandler.Implementation
{
    /// <summary>
    /// Author : Faris Jaafar
    /// Created Date : FEB 2019
    /// UPDATED : -
    /// VERSION 1.0
    /// </summary>
    public sealed class DBWrapper : IDisposable
    {
        private const int _defaultBulkBatch = 4999;
        private const string _system = "System";
        private DBWrapperFactory dbFactory;
        private IDbConnection _dbConnection;
        private IDBWrapper database;
        private readonly string providerName;
        private List<IDbDataParameter> parameters; //SO TIAP KALI USER CREATE INSTANCE ..AKAN RESET 
        private IDbCommand _dbCommand;
        public DBWrapper(string connectionStringName)
        {
            dbFactory = new DBWrapperFactory(connectionStringName);
            database = dbFactory.CreateDatabase();
            providerName = dbFactory.GetProviderName();
            parameters = new List<IDbDataParameter>(); //RESET
        }

        public DBWrapper(string ConString, string providerNames)
        {
            dbFactory = new DBWrapperFactory(ConString, providerNames);
            providerName = providerNames;
            database = dbFactory.CreateDatabase();
            parameters = new List<IDbDataParameter>(); //RESET
        }

        public void BeginTransaction()
        {
            try
            {
                database.Begin();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("error " + ex.Message);
            }
        }

        public void Commit()
        {
            try
            {
                database.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception("Error " + ex.Message);
            }
        }

        public void Rollback()
        {
            try
            {
                database.Rollback();
            }
            catch (Exception ex)
            {
                throw new Exception("error " + ex.Message);
            }
        }

        private IDbConnection GetDatabaseConnection()
        {
            _dbConnection = database.CreateConnection();
            return _dbConnection;
        }

        public void CreateParameter(string name, object value, DbType dbType)
        {
            parameters.Add(ParameterManager.CreateParameter(providerName, name, value, dbType, ParameterDirection.Input));
        }

        public void CreateParameter(string name, int size, object value, DbType dbType)
        {
            parameters.Add(ParameterManager.CreateParameter(providerName, name, size, value, dbType, ParameterDirection.Input));
        }

        public void CreateParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            parameters.Add(ParameterManager.CreateParameter(providerName, name, size, value, dbType, direction));
        }

        public void ClearParameters()
        {
            parameters.Clear();
            if (_dbCommand != null && _dbCommand.Parameters.Count > 0)
                _dbCommand.Parameters.Clear();
        }

        #region "get datatable"
        private DataTable GetDataTable(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);
                return dataset.Tables[0];
            }
        }

        public Task<DataTable> GetDataTableTextAsync(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);
                return Task.FromResult(dataset.Tables[0]);
            }
        }

        public DataTable GetDataTableText(string query)
        {
            return GetDataTable(query, CommandType.Text);
        }

        public DataTable GetDataTableSP(string spName)
        {
            return GetDataTable(spName, CommandType.StoredProcedure);
        }

        public async Task<DataTable> GetDataTableSPAsync(string query)
        {
            return await GetDataTableTextAsync(query, CommandType.StoredProcedure);
        }
        #endregion

        #region "get dataset"

        private DataSet GetDataSet(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);

                return dataset;
            }
        }

        private Task<DataSet> GetDataSetAsync(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);

                return Task.FromResult(dataset);
            }
        }

        public DataSet GetDataSetSP(string spName)
        {
            return GetDataSet(spName, CommandType.StoredProcedure);
        }

        public async Task<DataSet> GetDataSetSPAsync(string query)
        {
            return await GetDataSetAsync(query, CommandType.StoredProcedure);
        }

        public async Task<DataSet> GetDataSetTextAsync(string query)
        {
            return await GetDataSetAsync(query, CommandType.Text);
        }
        #endregion

        #region "get datareader"

        private DbDataReader GetDataReader(string commandText, CommandType commandType)
        {
            IDataReader reader = null;
            var connection = GetDatabaseConnection();
            _dbCommand = database.CreateCommand(commandText, commandType, connection);
            if (parameters.Count > 0)
            {
                Parallel.ForEach(parameters, (parameter) =>
                {
                    _dbCommand.Parameters.Add(parameter);
                });
            }

            reader = _dbCommand.ExecuteReader();
            return (DbDataReader)reader;
        }

        private Task<DbDataReader> GetDataReaderAsync(string commandText, CommandType commandType)
        {
            IDataReader reader = null;
            var connection = GetDatabaseConnection();
            _dbCommand = database.CreateCommand(commandText, commandType, connection);
            if (parameters.Count > 0)
            {
                Parallel.ForEach(parameters, (parameter) =>
                {
                    _dbCommand.Parameters.Add(parameter);
                });
            }

            reader = _dbCommand.ExecuteReader();
            return Task.FromResult((DbDataReader)reader);
        }

        public DbDataReader GetDataReaderSP(string spName)
        {
            return GetDataReader(spName, CommandType.StoredProcedure);
        }

        public DbDataReader GetDataReaderText(string query)
        {
            return GetDataReader(query, CommandType.StoredProcedure);
        }

        public async Task<DbDataReader> GetDataReaderSPAsync(string spName)
        {
            return await GetDataReaderAsync(spName, CommandType.StoredProcedure);
        }

        public async Task<DbDataReader> GetDataReaderTextAsync(string query)
        {
            return await GetDataReaderAsync(query, CommandType.Text);
        }
        #endregion

        #region "delete"
        private void Delete(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                _dbCommand.ExecuteNonQuery();
            }
        }

        private Task DeleteAsync(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                return Task.FromResult(_dbCommand.ExecuteNonQuery());
            }
        }

        public void DeleteSP(string spName)
        {
            Delete(spName, CommandType.StoredProcedure);
        }

        public void DeleteText(string query)
        {
            Delete(query, CommandType.Text);
        }


        public async Task DeleteSPAsync(string spName)
        {
            await DeleteAsync(spName, CommandType.StoredProcedure);
        }

        public async Task DeleteTextAsync(string query)
        {
            await DeleteAsync(query, CommandType.Text);
        }
        #endregion

        public int InsertWithReturn(string commandText, CommandType commandType, out int lastId)
        {
            int result = GetScalarValue<int>(commandText, commandType);
            lastId = Convert.ToInt32(result);
            return lastId;
            //var connection = GetDatabaseConnection();
            //using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            //{
            //    if (parameters.Count > 0)
            //    {
            //        Parallel.ForEach(parameters, (parameter) =>
            //        {
            //            _dbCommand.Parameters.Add(parameter);
            //        });
            //    }
            //    object newId = _dbCommand.ExecuteScalar();
            //    lastId = Convert.ToInt32(newId);
            //    return lastId;
            //}
        }

        public long InsertWithReturn(string commandText, CommandType commandType, out long lastId)
        {
            long result = GetScalarValue<long>(commandText, commandType);
            lastId = Convert.ToInt64(result);
            return lastId;
            //var connection = GetDatabaseConnection();
            //using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            //{
            //    if (parameters.Count > 0)
            //    {
            //        Parallel.ForEach(parameters, (parameter) =>
            //        {
            //            _dbCommand.Parameters.Add(parameter);
            //        });
            //    }
            //    object newId = _dbCommand.ExecuteScalar();
            //    lastId = Convert.ToInt64(newId);
            //    return lastId;
            //}
        }

        #region "Exec Non Query Command"
        public void ExecuteSP(string commandText)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, CommandType.StoredProcedure, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                _dbCommand.ExecuteNonQuery();
            }
        }

        public Task ExecuteSPAsync(string spName)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(spName, CommandType.StoredProcedure, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                return Task.FromResult(_dbCommand.ExecuteNonQuery());
            }
        }

        public void Execute(string commandText)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, CommandType.Text, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                _dbCommand.ExecuteNonQuery();
            }
        }

        public Task ExecuteAsync(string commandText)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, CommandType.Text, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                return Task.FromResult(_dbCommand.ExecuteNonQuery());
            }
        }
        #endregion

        public T GetScalarValueSP<T>(string commandText)
        {
            return GetScalarValue<T>(commandText, CommandType.StoredProcedure);
        }

        public T GetScalarValueText<T>(string commandText)
        {
            return GetScalarValue<T>(commandText, CommandType.Text);
        }

        /// <summary>
        /// Change to generic. easy to use
        /// </summary>
        /// <typeparam name="T">datatype</typeparam>
        /// <param name="commandText">direct sql or stor proc name ler..</param>
        /// <param name="commandType">CommandType</param>
        /// <returns>result int T</returns>
        private T GetScalarValue<T>(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                var result = _dbCommand.ExecuteScalar();
                if (Convert.IsDBNull(result))
                    return default(T);
                if (result is T) // just unbox
                    return (T)result;
                else            // convert
                    return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        public void BulkInsert<T>(string tableName, IList<T> list)
        {
            if (GetDatabaseConnection() is SqlConnection)
            {
                SqlConnection sqlConnection = (SqlConnection)GetDatabaseConnection();
                using (var bulkCopy = new SqlBulkCopy(sqlConnection))
                {
                    bulkCopy.BatchSize = _defaultBulkBatch;
                    bulkCopy.DestinationTableName = tableName;

                    var table = new DataTable();
                    var props = GetProps<T>();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];
                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }

                    bulkCopy.WriteToServer(table);
                }
            }
        }

        private static PropertyDescriptor[] GetProps<T>()
        {
            var props = TypeDescriptor.GetProperties(typeof(T))
                                       //Dirty hack to make sure we only have system data types 
                                       //i.e. filter out the relationships/collections
                                       .Cast<PropertyDescriptor>()
                                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals(_system))
                                       .ToArray();
            return props;
        }

        /// <summary>
        /// Faris.. clear all resourses!
        /// </summary>
        public void Dispose()
        {
            database.Dispose();
            if (_dbConnection != null)
                _dbConnection.Dispose();
            if (_dbCommand != null)
                _dbCommand.Dispose();
            _dbConnection = null;
            dbFactory = null;
            database = null;
            parameters = null;
            _dbCommand = null;
        }

        //public void InsertWithTransaction(string commandText, CommandType commandType)
        //{
        //    IDbTransaction transactionScope = null;
        //    using (var connection = GetDatabaseConnection())
        //    {

        //        transactionScope = connection.BeginTransaction();

        //        using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
        //        {
        //            if (parameters.Count > 0)
        //            {
        //                Parallel.ForEach(parameters, (parameter) =>
        //                {
        //                    _dbCommand.Parameters.Add(parameter);
        //                });
        //            }

        //            try
        //            {
        //                _dbCommand.ExecuteNonQuery();
        //                transactionScope.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                transactionScope.Rollback();
        //            }
        //            finally
        //            {
        //                connection.Dispose();
        //                _dbCommand.Parameters.Clear();
        //            }
        //        }
        //    }
        //}

        //public void InsertWithTransaction(string commandText, CommandType commandType, IsolationLevel isolationLevel)
        //{
        //    IDbTransaction transactionScope = null;
        //    using (var connection = GetDatabaseConnection())
        //    {

        //        transactionScope = connection.BeginTransaction(isolationLevel);

        //        using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
        //        {
        //            if (parameters.Count > 0)
        //            {
        //                Parallel.ForEach(parameters, (parameter) =>
        //                {
        //                    _dbCommand.Parameters.Add(parameter);
        //                });
        //            }

        //            try
        //            {
        //                _dbCommand.ExecuteNonQuery();
        //                transactionScope.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                transactionScope.Rollback();
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                _dbCommand.Parameters.Clear();
        //            }
        //        }
        //    }
        //}

        //public void UpdateWithTransaction(string commandText, CommandType commandType)
        //{
        //    IDbTransaction transactionScope = null;
        //    using (var connection = GetDatabaseConnection())
        //    {
        //        transactionScope = connection.BeginTransaction();

        //        using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
        //        {
        //            if (parameters.Count > 0)
        //            {
        //                Parallel.ForEach(parameters, (parameter) =>
        //                {
        //                    _dbCommand.Parameters.Add(parameter);
        //                });
        //            }

        //            try
        //            {
        //                _dbCommand.ExecuteNonQuery();
        //                transactionScope.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                transactionScope.Rollback();
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //    }
        //}

        //public void UpdateWithTransaction(string commandText, CommandType commandType, IsolationLevel isolationLevel)
        //{
        //    IDbTransaction transactionScope = null;
        //    using (var connection = GetDatabaseConnection())
        //    {
        //        transactionScope = connection.BeginTransaction(isolationLevel);

        //        using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
        //        {
        //            if (parameters.Count > 0)
        //            {
        //                Parallel.ForEach(parameters, (parameter) =>
        //                {
        //                    _dbCommand.Parameters.Add(parameter);
        //                });
        //            }

        //            try
        //            {
        //                _dbCommand.ExecuteNonQuery();
        //                transactionScope.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                transactionScope.Rollback();
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //    }
        //}
    }
}
