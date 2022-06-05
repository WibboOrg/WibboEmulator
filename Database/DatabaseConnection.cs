using Wibbo.Database.Adapter;
using Wibbo.Database.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;

namespace Wibbo.Database
{
    public class DatabaseConnection : IDatabaseClient, IDisposable
    {
        private readonly IQueryAdapter _adapter;
        private readonly MySqlConnection _con;

        public DatabaseConnection(string ConnectionStr)
        {
            this._con = new MySqlConnection(ConnectionStr);
            this._adapter = new NormaldbClient(this);
        }

        public void Dispose()
        {
            if (this._con.State == ConnectionState.Open)
            {
                this._con.Close();
            }

            this._con.Dispose();
            GC.SuppressFinalize(this);
        }

        public void connect()
        {
            this.Open();
        }

        public void disconnect()
        {
            this.Close();
        }

        public IQueryAdapter getQueryreactor()
        {
            return this._adapter;
        }

        public void reportDone()
        {
            this.Dispose();
        }

        public MySqlCommand createNewCommand()
        {
            return this._con.CreateCommand();
        }

        public void Open()
        {
            if (this._con.State == ConnectionState.Closed)
            {
                try
                {
                    this._con.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void Close()
        {
            if (this._con.State == ConnectionState.Open)
            {
                this._con.Close();
            }
        }
    }
}