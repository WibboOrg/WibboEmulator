namespace WibboEmulator.Database;
using MySql.Data.MySqlClient;
using WibboEmulator.Database.Interfaces;

public sealed class DatabaseManager
{
    private readonly string _connectionStr;

    public DatabaseManager(DatabaseConfiguration databaseConfiguration)
    {
        var connectionString = new MySqlConnectionStringBuilder
        {
            ConnectionTimeout = 10,
            Database = databaseConfiguration.Name,
            DefaultCommandTimeout = 30,
            Logging = false,
            MaximumPoolSize = databaseConfiguration.MaximumPoolSize,
            MinimumPoolSize = databaseConfiguration.MinimumPoolSize,
            Password = databaseConfiguration.Password,
            Pooling = true,
            Port = databaseConfiguration.Port,
            Server = databaseConfiguration.Hostname,
            UserID = databaseConfiguration.Username,
            AllowZeroDateTime = true
        };

        this._connectionStr = connectionString.ToString();
    }

    public bool IsConnected()
    {
        try
        {
            var Con = new MySqlConnection(this._connectionStr);
            Con.Open();
            var CMD = Con.CreateCommand();
            CMD.CommandText = "SELECT 1+1";
            CMD.ExecuteNonQuery();
            CMD.Dispose();
            Con.Close();
        }
        catch (MySqlException)
        {
            return false;
        }

        return true;
    }

    public IQueryAdapter GetQueryReactor()
    {
        try
        {
            IDatabaseClient DbConnection = new DatabaseConnection(this._connectionStr);

            DbConnection.Connect();

            return DbConnection.GetQueryreactor();
        }
        catch (Exception e)
        {
            ExceptionSQL.LogException(e.ToString());
            return null;
        }
    }
}