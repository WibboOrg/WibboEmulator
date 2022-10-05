namespace WibboEmulator.Database.Daos;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class WordFilterRetroDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `word` FROM `word_filter_retro`");
        return dbClient.GetTable();
    }

    internal static void Insert(IQueryAdapter dbClient, string word)
    {
        dbClient.SetQuery("INSERT INTO `word_filter_retro` (`word`) VALUES (@word)");
        dbClient.AddParameter("word", word);
        dbClient.RunQuery();
    }
}