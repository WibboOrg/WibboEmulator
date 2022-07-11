using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Adapter
{
    public class NormaldbClient : QueryAdapter, IQueryAdapter, IRegularQueryAdapter, IDisposable
    {
        public NormaldbClient(IDatabaseClient Client)
            : base(Client)
        {
            command = Client.createNewCommand();
        }

        public void Dispose()
        {
            command.Dispose();
            client.reportDone();
            GC.SuppressFinalize(this);
        }
    }
}