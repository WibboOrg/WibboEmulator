namespace WibboEmulator.Database.Adapter;
using WibboEmulator.Database.Interfaces;

public class NormaldbClient : QueryAdapter, IQueryAdapter, IRegularQueryAdapter, IDisposable
{
    public NormaldbClient(IDatabaseClient client)
        : base(client) => this.Command = client.CreateNewCommand();

    public void Dispose()
    {
        this.Command.Dispose();
        this.Client.Dispose();
        GC.SuppressFinalize(this);
    }
}
