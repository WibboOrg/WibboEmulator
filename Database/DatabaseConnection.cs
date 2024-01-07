namespace WibboEmulator.Database;
using System.Data;
using MySql.Data.MySqlClient;
using WibboEmulator.Core;
using WibboEmulator.Database.Adapter;
using WibboEmulator.Database.Interfaces;

public class DatabaseConnection : IDatabaseClient, IDisposable
{
    private readonly IQueryAdapter _adapter;
    private readonly MySqlConnection _con;

    public string ConnectionStr { get; }

    public DatabaseConnection(string connectionStr)
    {
        this._con = new MySqlConnection(connectionStr);
        this._adapter = new NormaldbClient(this);
        this.ConnectionStr = connectionStr;
    }

    public void Dispose()
    {
        this.Close();

        this._con.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Connect() => this.Open();

    public void Disconnect() => this.Close();

    public IQueryAdapter GetQueryReactor() => this._adapter;

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
                ExceptionLogger.LogException($"Database connection Open error: {e.Message}");
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
