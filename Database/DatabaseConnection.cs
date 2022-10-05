namespace WibboEmulator.Database;
using System.Data;
using MySql.Data.MySqlClient;
using WibboEmulator.Database.Adapter;
using WibboEmulator.Database.Interfaces;

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

    public void Connect() => this.Open();

    public void Disconnect() => this.Close();

    public IQueryAdapter GetQueryreactor() => this._adapter;

    public void ReportDone() => this.Dispose();

    public MySqlCommand CreateNewCommand() => this._con.CreateCommand();

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