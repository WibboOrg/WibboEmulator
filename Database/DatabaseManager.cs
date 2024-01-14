namespace WibboEmulator.Database;

using System.Data;
using MySqlConnector;

public sealed class DatabaseManager
{
    private readonly string _connectionStr;

    public DatabaseManager(DatabaseConfiguration databaseConfiguration)
    {
        var connectionStringBuilder = new MySqlConnectionStringBuilder
        {
            ConnectionTimeout = 300,
            Database = databaseConfiguration.Name,
            DefaultCommandTimeout = 300,
            MaximumPoolSize = databaseConfiguration.MaximumPoolSize,
            MinimumPoolSize = databaseConfiguration.MinimumPoolSize,
            Password = databaseConfiguration.Password,
            Pooling = true,
            Port = databaseConfiguration.Port,
            Server = databaseConfiguration.Hostname,
            UserID = databaseConfiguration.Username,
            AllowZeroDateTime = true
        };

        this._connectionStr = connectionStringBuilder.ToString();

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public bool IsConnected()
    {
        try
        {
            using var con = this.Connection();
            con.Open();
            using var cmd = con.CreateCommand();
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

    public IDbConnection Connection() => new MySqlConnection(this._connectionStr);
}
