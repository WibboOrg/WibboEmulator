using Butterfly.Database.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace Butterfly.Utilities
{
    public class QueryChunk
    {
        private Dictionary<string, string> parameters;
        private StringBuilder queries;
        private int queryCount;
        private readonly EndingType endingType;

        public QueryChunk()
        {
            this.parameters = new Dictionary<string, string>();
            this.queries = new StringBuilder();
            this.queryCount = 0;
            this.endingType = EndingType.SEQUENTIAL;
        }

        public QueryChunk(string startQuery)
        {
            this.parameters = new Dictionary<string, string>();
            this.queries = new StringBuilder(startQuery);
            this.endingType = EndingType.CONTINUOUS;
            this.queryCount = 0;
        }

        public void AddQuery(string query)
        {
            ++this.queryCount;
            this.queries.Append(query);
            switch (this.endingType)
            {
                case EndingType.SEQUENTIAL:
                    this.queries.Append(";");
                    break;
                case EndingType.CONTINUOUS:
                    this.queries.Append(",");
                    break;
            }
        }

        public void AddParameter(string parameterName, string value)
        {
            this.parameters.Add(parameterName, value);
        }

        public void Execute(IQueryAdapter dbClient)
        {
            if (this.queryCount == 0)
            {
                return;
            }

            this.queries = this.queries.Remove(this.queries.Length - 1, 1);
            dbClient.SetQuery((this.queries).ToString());
            foreach (KeyValuePair<string, string> keyValuePair in this.parameters)
            {
                dbClient.AddParameter(keyValuePair.Key, keyValuePair.Value);
            }

            dbClient.RunQuery();
        }

        public void Dispose()
        {
            this.parameters.Clear();
            this.queries.Clear();
            this.parameters = null;
            this.queries = null;
        }
    }
}
