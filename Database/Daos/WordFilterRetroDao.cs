using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class WordFilterRetroDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT word FROM word_filter_retro");
            DataTable Data2 = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO word_filter_retro (word) VALUES (@word)");
            dbClient.AddParameter("word", Word);
            dbClient.RunQuery();
        }
    }
}