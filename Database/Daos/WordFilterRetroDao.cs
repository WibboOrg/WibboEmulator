namespace WibboEmulator.Database.Daos;

using System.Data;
using Dapper;

internal sealed class WordFilterRetroDao
{
    internal static List<string> GetAll(IDbConnection dbClient) => dbClient.Query<string>(
        "SELECT `word` FROM `word_filter_retro`"
    ).ToList();

    internal static void Insert(IDbConnection dbClient, string word) => dbClient.Execute(
        "INSERT INTO `word_filter_retro` (`word`) VALUES (@word)",
        new { word });
}