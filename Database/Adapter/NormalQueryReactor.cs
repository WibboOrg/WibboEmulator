using Butterfly.Database.Interfaces;
using System;

namespace Butterfly.Database.Adapter
{
    public class NormaldbClient : QueryAdapter, IQueryAdapter, IRegularQueryAdapter, IDisposable
    {
        public NormaldbClient(IDatabaseClient Client)
            : base(Client)
        {
            this.command = Client.createNewCommand();
        }

        public void Dispose()
        {
            this.command.Dispose();
            this.client.reportDone();
            GC.SuppressFinalize(this);
        }
    }
}