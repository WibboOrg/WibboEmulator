using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class WordFilterRetroDao
    {
        internal static DataTable Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT word FROM word_filter_retro");
            return dbClient.GetTable();
        }

        internal static void Insert(IQueryAdapter dbClient, string word)
        {
            dbClient.SetQuery("INSERT INTO word_filter_retro (word) VALUES (@word)");
            dbClient.AddParameter("word", word);
            dbClient.RunQuery();
        }
    }
}