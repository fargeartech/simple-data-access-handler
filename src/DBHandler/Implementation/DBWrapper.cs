using DBHandler.DbProvider;
using DBHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DBHandler.Implementation
{
    /// <summary>
    /// Author : Faris Jaafar
    /// Created Date : FEB 2019
    /// UPDATED : -
    /// VERSION 1.0
    /// </summary>
    public class DBWrapper : IDisposable
    {
        private DBWrapperFactory dbFactory;
        private IDbConnection _dbConnection;
        private IDBWrapper database;
        private string providerName;
        private List<IDbDataParameter> parameters; //SO TIAP KALI USER CREATE INSTANCE ..AKAN RESET 
        public DBWrapper(string connectionStringName)
        {
            dbFactory = new DBWrapperFactory(connectionStringName);
            database = dbFactory.CreateDatabase();
            providerName = dbFactory.GetProviderName();
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

        }

        public DataTable GetDataTable(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (var command = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                     {
                         command.Parameters.Add(parameter);
                     });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);
                return dataset.Tables[0];
            }
        }

        public DataSet GetDataSet(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (var command = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        command.Parameters.Add(parameter);
                    });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);

                return dataset;
            }
        }

        public IDataReader GetDataReader(string commandText, CommandType commandType)
        {
            IDataReader reader = null;
            var connection = GetDatabaseConnection();
            var command = database.CreateCommand(commandText, commandType, connection);
            if (parameters.Count > 0)
            {
                Parallel.ForEach(parameters, (parameter) =>
                {
                    command.Parameters.Add(parameter);
                });
            }

            reader = command.ExecuteReader();
            return reader;
        }

        public void Delete(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (var command = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        command.Parameters.Add(parameter);
                    });
                }
                command.ExecuteNonQuery();
            }
        }

        public void Insert(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (var command = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        command.Parameters.Add(parameter);
                    });
                }
                command.ExecuteNonQuery();
            }
        }

        public int Insert(string commandText, CommandType commandType, out int lastId)
        {
            lastId = 0;
            var connection = GetDatabaseConnection();
            using (var command = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        command.Parameters.Add(parameter);
                    });
                }
                object newId = command.ExecuteScalar();
                lastId = Convert.ToInt32(newId);

                return lastId;
            }
        }

        public long Insert(string commandText, CommandType commandType, out long lastId)
        {
            lastId = 0;
            var connection = GetDatabaseConnection();
            using (var command = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        command.Parameters.Add(parameter);
                    });
                }
                object newId = command.ExecuteScalar();
                lastId = Convert.ToInt64(newId);
                return lastId;
            }
        }

        public void InsertWithTransaction(string commandText, CommandType commandType)
        {
            IDbTransaction transactionScope = null;
            using (var connection = GetDatabaseConnection())
            {

                transactionScope = connection.BeginTransaction();

                using (var command = database.CreateCommand(commandText, commandType, connection))
                {
                    if (parameters.Count > 0)
                    {
                        Parallel.ForEach(parameters, (parameter) =>
                        {
                            command.Parameters.Add(parameter);
                        });
                    }

                    try
                    {
                        command.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        public void InsertWithTransaction(string commandText, CommandType commandType, IsolationLevel isolationLevel)
        {
            IDbTransaction transactionScope = null;
            using (var connection = GetDatabaseConnection())
            {

                transactionScope = connection.BeginTransaction(isolationLevel);

                using (var command = database.CreateCommand(commandText, commandType, connection))
                {
                    if (parameters.Count > 0)
                    {
                        Parallel.ForEach(parameters, (parameter) =>
                        {
                            command.Parameters.Add(parameter);
                        });
                    }

                    try
                    {
                        command.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public void Update(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (var command = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        command.Parameters.Add(parameter);
                    });
                }
                command.ExecuteNonQuery();
            }
        }

        public void UpdateWithTransaction(string commandText, CommandType commandType)
        {
            IDbTransaction transactionScope = null;
            using (var connection = GetDatabaseConnection())
            {
                transactionScope = connection.BeginTransaction();

                using (var command = database.CreateCommand(commandText, commandType, connection))
                {
                    if (parameters.Count > 0)
                    {
                        Parallel.ForEach(parameters, (parameter) =>
                        {
                            command.Parameters.Add(parameter);
                        });
                    }

                    try
                    {
                        command.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public void UpdateWithTransaction(string commandText, CommandType commandType, IsolationLevel isolationLevel)
        {
            IDbTransaction transactionScope = null;
            using (var connection = GetDatabaseConnection())
            {
                transactionScope = connection.BeginTransaction(isolationLevel);

                using (var command = database.CreateCommand(commandText, commandType, connection))
                {
                    if (parameters.Count > 0)
                    {
                        Parallel.ForEach(parameters, (parameter) =>
                        {
                            command.Parameters.Add(parameter);
                        });
                    }

                    try
                    {
                        command.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Change to generic. easy to use
        /// </summary>
        /// <typeparam name="T">datatype</typeparam>
        /// <param name="commandText">direct sql or stor proc name ler..</param>
        /// <param name="commandType">CommandType</param>
        /// <returns>result int T</returns>
        public T GetScalarValue<T>(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (var command = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        command.Parameters.Add(parameter);
                    });
                }
                var result = command.ExecuteScalar();
                if (Convert.IsDBNull(result))
                    return default(T);
                if (result is T) // just unbox
                    return (T)result;
                else            // convert
                    return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        public DataTable ExecuteDatatableSQL(string sql)
        {
            var connection = GetDatabaseConnection();
            using (var command = database.CreateCommand(sql, CommandType.Text, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        command.Parameters.Add(parameter);
                    });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);
                return dataset.Tables[0];
            }

        }
        public IDataReader ExecuteDataReaderSQL(string commandText)
        {
            IDataReader reader = null;
            var connection = GetDatabaseConnection();
            var command = database.CreateCommand(commandText, CommandType.Text, connection);
            if (parameters.Count > 0)
            {
                Parallel.ForEach(parameters, (parameter) =>
                {
                    command.Parameters.Add(parameter);
                });
            }
            reader = command.ExecuteReader();

            return reader;
        }
        /// <summary>
        /// Faris.. clear all resourses!
        /// </summary>
        public void Dispose()
        {
            database.Dispose();
            if (_dbConnection != null)
                _dbConnection.Dispose();
            _dbConnection = null;
            dbFactory = null;
            database = null;
            parameters = null;
        }
    }
}
