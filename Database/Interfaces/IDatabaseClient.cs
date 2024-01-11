namespace WibboEmulator.Database.Interfaces;
using MySqlConnector;

public interface IDatabaseClient : IDisposable
{
    void Open();
    void Close();
    IQueryAdapter GetQueryReactor();
    MySqlCommand CreateNewCommand();
}
