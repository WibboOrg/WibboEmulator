using MySql.Data.MySqlClient;

namespace Butterfly.Database.Interfaces
{
    public interface IDatabaseClient : IDisposable
    {
        void connect();
        void disconnect();
        IQueryAdapter getQueryreactor();
        MySqlCommand createNewCommand();
        void reportDone();
    }
}