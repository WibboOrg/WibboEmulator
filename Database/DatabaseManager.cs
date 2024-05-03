namespace WibboEmulator.Database;

using System.Data;
using Dapper;
using MySqlConnector;

public static class DatabaseManager
{
    private static string _connectionString;

    public static void Initialize(DatabaseConfiguration databaseConfiguration)
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

        _connectionString = connectionStringBuilder.ToString();

        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public static bool IsConnected
    {
        get
        {
            try
            {
                using var con = Connection;
                _ = con.Execute("SELECT 1+1");
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
        }
    }

    public static IDbConnection Connection => new MySqlConnection(_connectionString);
}
