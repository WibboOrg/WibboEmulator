namespace WibboEmulator.Database.Interfaces;
using MySql.Data.MySqlClient;

public interface IDatabaseClient : IDisposable
{
    void Connect();
    void Disconnect();
    IQueryAdapter GetQueryReactor();
    MySqlCommand CreateNewCommand();
    void ReportDone();
}
