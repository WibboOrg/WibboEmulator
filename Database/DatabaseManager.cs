namespace WibboEmulator.Database;

using System.Data;
using MySql.Data.MySqlClient;
using WibboEmulator.Core;
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
            var con = new MySqlConnection(this._connectionStr);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT 1+1";
            _ = cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
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
            IDatabaseClient dbConnection = new DatabaseConnection(this._connectionStr);

            dbConnection.Connect();

            return dbConnection.GetQueryreactor();
        }
        catch (Exception e)
        {
            ExceptionLogger.LogException(e.ToString());
            return null;
        }
    }

    public IDbConnection Connection() => new MySqlConnection(this._connectionStr);
}
