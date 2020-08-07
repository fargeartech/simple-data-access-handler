# A Simple Data Access Handler for ADO.NET
A simple data access layer to access **different** type of database. A simple wrapper will handle will support normal connection including transaction.

## Supported Database
 >SQL Server MySQL SQLite

## Updated Soon
> Oracle , ODBC and OLEDB

## How To Use
Always wrap the DBWrapper by utilizing *using statement* to ensure all connection has been dispose properly to prevent memory leak.

<pre lang=c#>
  using (var dal = new DBWrapper("DBConnection"))
</pre>

**DBConnection** is your webconfig connection name where this library will automatically find **connectionstring and provider** name.
```xml
 <connectionStrings>
    <add name="DBConnection" connectionString="Data Source=localhost;Initial Catalog=demo;Integrated Security=True" providerName="System.Data.SqlClient" />
    <add name="DBSQLite" connectionString="Data Source=mydb.db;Version=3;" providerName="System.Data.Sqlite" />
  </connectionStrings>
```

you will have the flexibility to use *Stor Proc* or *Direct Query* which require your needs.

## using stor proc
<pre lang=c#>
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

                    int count = dal.GetScalarValue<int>("MY_STOR_PROC_NAME", CommandType.StoredProcedure);
                    Console.WriteLine("current total " + count.ToString());
           -- skip for brevity
</pre>

## using direct query
<pre lang=c#>
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

                    int count = dal.GetScalarValue<int>("select count(*) from table", CommandType.Text);
                    Console.WriteLine("current total " + count.ToString());
           -- skip for brevity
</pre>

for full example you can refer [C# Example](https://github.com/fargeartech/simple-data-access-handler/tree/master/src/DbHandler.Console) and for **VB.NET**
[VB.NET Example](https://github.com/fargeartech/simple-data-access-handler/tree/master/src/DBHanlderVB.ConsoleApp)


Thank you and happy Coding :heartpulse: from **Malaysia**
