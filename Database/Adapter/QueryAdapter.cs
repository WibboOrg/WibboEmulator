using Butterfly.Core;
using Butterfly.Database.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;

namespace Butterfly.Database.Adapter
{
    public class QueryAdapter : IRegularQueryAdapter
    {
        protected IDatabaseClient client;
        protected MySqlCommand command;


        public bool dbEnabled = true;
        public QueryAdapter(IDatabaseClient Client)
        {
            this.client = Client;
        }

        public void AddParameter(string parameterName, string val)
        {
            this.command.Parameters.AddWithValue(parameterName, val);
        }

        public void AddParameter(string parameterName, int val)
        {
            this.command.Parameters.AddWithValue(parameterName, val.ToString());
        }

        public bool FindsResult()
        {
            bool hasRows = false;
            try
            {
                using (MySqlDataReader reader = this.command.ExecuteReader())
                {
                    hasRows = reader.HasRows;
                }
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.command.CommandText);
            }

            return hasRows;
        }

        public int GetInteger()
        {
            int result = 0;
            try
            {
                object obj2 = this.command.ExecuteScalar();
                if (obj2 != null)
                {
                    int.TryParse(obj2.ToString(), out result);
                }
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.command.CommandText);
            }

            return result;
        }

        public DataRow GetRow()
        {
            DataRow row = null;
            try
            {
                DataSet dataSet = new DataSet();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(this.command))
                {
                    adapter.Fill(dataSet);
                }
                if ((dataSet.Tables.Count > 0) && (dataSet.Tables[0].Rows.Count == 1))
                {
                    row = dataSet.Tables[0].Rows[0];
                }
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.command.CommandText);
            }

            return row;
        }

        public string GetString()
        {
            string str = string.Empty;
            try
            {
                object obj2 = this.command.ExecuteScalar();
                if (obj2 != null)
                {
                    str = obj2.ToString();
                }
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.command.CommandText);
            }

            return str;
        }

        public DataTable GetTable()
        {
            DataTable dataTable = new DataTable();
            if (!this.dbEnabled)
            {
                return dataTable;
            }

            try
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(this.command))
                {
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.command.CommandText);
            }

            return dataTable;
        }

        public void RunQuery(string query)
        {
            if (!this.dbEnabled)
            {
                return;
            }

            this.SetQuery(query);
            this.RunQuery();
        }

        public void SetQuery(string query)
        {
            this.command.Parameters.Clear();
            this.command.CommandText = query;
        }

        public long InsertQuery()
        {
            if (!this.dbEnabled)
            {
                return 0;
            }

            long lastInsertedId = 0L;
            try
            {
                this.command.ExecuteScalar();
                lastInsertedId = this.command.LastInsertedId;
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.command.CommandText);
            }
            return lastInsertedId;
        }

        public void RunQuery()
        {
            if (!this.dbEnabled)
            {
                return;
            }

            try
            {
                this.command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.command.CommandText);
            }
        }
    }
}