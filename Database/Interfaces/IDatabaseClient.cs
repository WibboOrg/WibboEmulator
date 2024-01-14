namespace System.Data;
using MySqlConnector;

public interface IDatabaseClient : IDisposable
{
    void Open();
    void Close();
    IDbConnection Connection();
    MySqlCommand CreateNewCommand();
}
