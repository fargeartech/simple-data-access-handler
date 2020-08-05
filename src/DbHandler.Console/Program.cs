using DBHandler.Implementation;
using System;
using System.Data;

namespace DbHandler.ConsoleApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("starting ---");
            RunSQLite();
            //RunSQLServer();
        }
        private static void RunSQLServer()
        {
            using (var dal = new DBWrapper("DBConnection"))
            {
                try
                {
                    var user = new User
                    {
                        FirstName = "First",
                        LastName = "Last",
                        Dob = DateTime.Now.AddDays(-3000),
                        IsActive = true
                    };

                    int count = dal.GetScalarValue<int>("DAH_User_Scalar", CommandType.StoredProcedure);
                    Console.WriteLine("current total " + count.ToString());
                    AddLog("insert user with trans ---");
                    dal.BeginTransaction();
                    AddLog("begin trans ---");
                    dal.CreateParameter("@FirstName", 50, user.FirstName, DbType.String);
                    dal.CreateParameter("@LastName", user.LastName, DbType.String);
                    dal.CreateParameter("@Dob", user.Dob, DbType.DateTime);
                    dal.CreateParameter("@IsActive", 50, user.IsActive, DbType.Boolean);
                    int newId;
                    dal.Insert("DAH_User_Insert", CommandType.StoredProcedure, out newId);
                    dal.Commit();
                    AddLog("done commit ---");
                    AddLog("new user id " + newId.ToString());
                    dal.ClearParameters();
                    int count_1 = dal.GetScalarValue<int>("DAH_User_Scalar", CommandType.StoredProcedure);
                    AddLog("current total " + count_1.ToString());

                    dal.ClearParameters();
                    dal.CreateParameter("@Id", 12, DbType.Int32);
                    using (var reader = dal.GetDataReader("DAH_User_GetById", CommandType.StoredProcedure))
                    {
                        while (reader.Read())
                        {
                            AddLog(string.Format("user id 12 :  firstname {0}", reader["FirstName"].ToString()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    try //https://docs.microsoft.com/en-us/archive/blogs/dataaccesstechnologies/zombie-check-on-transaction-error-this-sqltransaction-has-completed-it-is-no-longer-usable
                    {
                        dal.Rollback();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }
        private static void RunSQLite()
        {
            using (var dal = new DBWrapper("DBSQLite"))
            {
                try
                {
                    var user = new User
                    {
                        FirstName = "First",
                        LastName = "Last",
                        Dob = DateTime.Now.AddDays(-3000),
                        IsActive = true
                    };

                    using (var reader = dal.ExecuteDataReaderSQL("select * from users"))
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("loop users " + reader["FirstName"]);
                        }
                    }

                    int count = dal.GetScalarValue<int>("select count(*) from users", CommandType.Text);
                    Console.WriteLine("count " + count.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("error " + ex.Message);
                }
            }
        }
        private static void AddLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
