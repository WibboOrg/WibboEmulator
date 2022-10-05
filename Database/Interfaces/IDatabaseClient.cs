namespace WibboEmulator.Database.Interfaces;
using MySql.Data.MySqlClient;

public interface IDatabaseClient : IDisposable
{
    void Connect();
    void Disconnect();
    IQueryAdapter GetQueryreactor();
    MySqlCommand CreateNewCommand();
    void ReportDone();
}
