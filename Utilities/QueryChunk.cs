namespace WibboEmulator.Utilities;
using System.Text;
using WibboEmulator.Database.Interfaces;

public class QueryChunk
{
    private Dictionary<string, string> _parameters;
    private StringBuilder _queries;
    private int _queryCount;
    private readonly EndingType _endingType;

    public QueryChunk()
    {
        this._parameters = new Dictionary<string, string>();
        this._queries = new StringBuilder();
        this._queryCount = 0;
        this._endingType = EndingType.SEQUENTIAL;
    }

    public QueryChunk(string startQuery)
    {
        this._parameters = new Dictionary<string, string>();
        this._queries = new StringBuilder(startQuery);
        this._endingType = EndingType.CONTINUOUS;
        this._queryCount = 0;
    }

    public void AddQuery(string query)
    {
        ++this._queryCount;
        _ = this._queries.Append(query);
        switch (this._endingType)
        {
            case EndingType.SEQUENTIAL:
                _ = this._queries.Append(';');
                break;
            case EndingType.CONTINUOUS:
                _ = this._queries.Append(',');
                break;
        }
    }

    public void AddParameter(string parameterName, string value) => this._parameters.Add(parameterName, value);

    public void Execute(IQueryAdapter dbClient)
    {
        if (this._queryCount == 0)
        {
            return;
        }

        this._queries = this._queries.Remove(this._queries.Length - 1, 1);
        dbClient.SetQuery(this._queries.ToString());
        foreach (var keyValuePair in this._parameters)
        {
            dbClient.AddParameter(keyValuePair.Key, keyValuePair.Value);
        }

        dbClient.RunQuery();
    }

    public void Dispose()
    {
        this._parameters.Clear();
        _ = this._queries.Clear();
        this._parameters = null;
        this._queries = null;
    }
}
