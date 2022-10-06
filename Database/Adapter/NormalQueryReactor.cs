namespace WibboEmulator.Database.Adapter;
using WibboEmulator.Database.Interfaces;

public class NormaldbClient : QueryAdapter, IQueryAdapter, IRegularQueryAdapter, IDisposable
{
    public NormaldbClient(IDatabaseClient Client)
        : base(Client) => this.Command = Client.CreateNewCommand();

    public void Dispose()
    {
        this.Command.Dispose();
        this.Client.ReportDone();
        GC.SuppressFinalize(this);
    }
}
