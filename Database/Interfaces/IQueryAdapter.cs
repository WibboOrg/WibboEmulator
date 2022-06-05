namespace Wibbo.Database.Interfaces
{
    public interface IQueryAdapter : IRegularQueryAdapter, IDisposable
    {
        long InsertQuery();
    }
}