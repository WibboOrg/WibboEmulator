namespace WibboEmulator.Database.Daos.Moderation;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class ModerationTopicDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `id`, `caption` FROM `moderation_topic`");
        return dbClient.GetTable();
    }
}