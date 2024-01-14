namespace System.Data;

public interface IDbConnectionaa : IRegularQueryAdapter, IDisposable
{
    long InsertQuery();
}