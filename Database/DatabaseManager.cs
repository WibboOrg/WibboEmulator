using WibboEmulator.Database.Interfaces;
using MySql.Data.MySqlClient;
using Plus.Database;

namespace WibboEmulator.Database
{
    public sealed class DatabaseManager
    {
        private readonly string _connectionStr;

        public DatabaseManager(DatabaseConfiguration databaseConfiguration)
        {
            MySqlConnectionStringBuilder connectionString = new MySqlConnectionStringBuilder
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
                MySqlConnection Con = new MySqlConnection(this._connectionStr);
                Con.Open();
                MySqlCommand CMD = Con.CreateCommand();
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

                DbConnection.connect();

                return DbConnection.getQueryreactor();
            }
            catch (Exception e)
            {
                ExceptionSQL.LogException(e.ToString());
                return null;
            }
        }
    }
}