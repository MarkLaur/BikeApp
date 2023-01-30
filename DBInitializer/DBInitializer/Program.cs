using MySql.Data.MySqlClient;
using Mysqlx.Expr;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.Crmf;
using System.Diagnostics;
using System.Diagnostics.Metrics;

internal class Program
{
    private enum DBConnectionTestResult
    {
        /// <summary>
        /// Main connection test was succesful.
        /// </summary>
        Success,
        /// <summary>
        /// Main connection test failed but default connection test was succesful.
        /// </summary>
        DefaultConnectionSuccess,
        /// <summary>
        /// All connection tests failed.
        /// </summary>
        Failure,
    }

    private const string customRootPW = "BikeAppUsbwpw";
    private const string dbName = "bikeapp";

    //Without database
    private const string connectionStringNoDB = $"Server=localhost; Port=3307; Uid=root; Pwd={customRootPW};";
    private const string uswbDefaultConnectionStringNoDB = "Server=localhost; Port=3307; Uid=root; Pwd=usbw;";

    //With database
    private const string connectionString = $"Server=localhost; Port=3307; Database={dbName}; Uid=root; Pwd={customRootPW};";
    //private const string uswbDefaultConnectionString = "Server=localhost; Port=3307; Database=test; Uid=root; Pwd=usbw;";

    private const string checkTableCommand = "show tables like @tablename";

    private static MySqlConnection conn = new MySqlConnection();

    private static void Main(string[] args)
    {
        InitializeDB();

        //Keep window open
        Console.ReadKey();
    }

    private static void InitializeDB()
    {
        //Setup user
        if (TryOpenConnection(connectionStringNoDB, out string error))
        {
            Console.WriteLine("User is already setup.");
        }
        else if (TryOpenConnection(uswbDefaultConnectionStringNoDB, out error))
        {
            Console.WriteLine($"Logged in as default root user. Changing root password. {error}");

            RunCommand($"SET PASSWORD FOR root@localhost = PASSWORD('{customRootPW}'); FLUSH PRIVILEGES;");
            conn.Close();

            if (!TryOpenConnection(connectionStringNoDB, out error))
            {
                Console.WriteLine($"Something went wrong with password change. {error}");
            }
        }
        else
        {
            Console.WriteLine($"All connection tests failed. Cannot initialize database. Default test settings: {uswbDefaultConnectionStringNoDB}\n{error}");
            return;
        }

        //Setup database
        if (DatabaseExists(dbName))
        {
            Console.WriteLine("Bikeapp database already exists.");
        }
        else
        {
            Console.WriteLine("Database is missing. Adding bikeapp database.");
            RunCommand($"CREATE DATABASE {dbName};");
        }

        conn.Close();

        //Test and open final connection
        if (TryOpenConnection(connectionString, out error))
        {
            Console.WriteLine("Bike app user and database ok.");
        }
        else
        {
            Console.WriteLine($"Bike app user or database is still not setup. Something has gone horribly wrong.\n {error}");
            return;
        }

        //Get project folder
        string? projectFolder = AppContext.BaseDirectory;

        //Test and add bikestations table
        if (TestTable("bikestations"))
        {
            Console.WriteLine("Bikestations table exists.");
            //TODO: test table layout and overwrite if needed
        }
        else
        {
            Console.WriteLine("Bikestations table not found. Creating it.");

            string tableCommand;
            if (string.IsNullOrEmpty(projectFolder))
            {
                tableCommand = File.ReadAllText("bikestations.sql");
            }
            else
            {
                tableCommand = File.ReadAllText(Path.Combine(projectFolder, "bikestations.sql"));
            }

            RunCommand(tableCommand);
        }

        //Test and add biketrips table
        if (TestTable("biketrips"))
        {
            Console.WriteLine("Biketrips table exists.");
            //TODO: test table layout and overwrite if needed
        }
        else
        {
            Console.WriteLine("Biketrips table not found. Creating it.");

            string tableCommand;
            if (string.IsNullOrEmpty(projectFolder))
            {
                tableCommand = File.ReadAllText("biketrips.sql");
            }
            else
            {
                tableCommand = File.ReadAllText(Path.Combine(projectFolder, "biketrips.sql"));
            }

            RunCommand(tableCommand);
        }

        Console.WriteLine("Database initialized. You can now close this window.");
    }

    private static bool TryOpenConnection(string connectionString, out string error)
    {
        //Try to open main connection
        try
        {
            conn.ConnectionString = connectionString;
            conn.Open();
            error = "";
            return true;
        }
        catch(Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private static bool TestTable(string tableName)
    {
        using (MySqlCommand command = conn.CreateCommand())
        {
            command.CommandText = checkTableCommand;
            command.Parameters.AddWithValue("@tablename", tableName);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                return reader.HasRows;
            }
        }
    }

    private static bool DatabaseExists(string database)
    {
        try
        {
            RunCommand($"USE {database}");
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void RunCommand(string command)
    {
        using (MySqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = command;
            cmd.ExecuteNonQuery();
        }
    }
}