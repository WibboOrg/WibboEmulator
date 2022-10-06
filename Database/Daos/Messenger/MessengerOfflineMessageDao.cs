namespace WibboEmulator.Database.Daos.Messenger;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class MessengerOfflineMessageDao
{
    internal static void Insert(IQueryAdapter dbClient, int toId, int userId, string message)
    {
        dbClient.SetQuery("INSERT INTO `messenger_offline_message` (`to_id`, `from_id`, `message`, `timestamp`) VALUES (@tid, @fid, @msg, UNIX_TIMESTAMP())");
        dbClient.AddParameter("tid", toId);
        dbClient.AddParameter("fid", userId);
        dbClient.AddParameter("msg", message);
        dbClient.RunQuery();
    }

    internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT `id`, `to_id`, `from_id`, `message`, `timestamp` FROM `messenger_offline_message` WHERE `to_id` = @id");
        dbClient.AddParameter("id", userId);
        return dbClient.GetTable();
    }

    internal static void Delete(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("DELETE FROM `messenger_offline_message` WHERE `to_id` = @id");
        dbClient.AddParameter("id", userId);
        dbClient.RunQuery();
    }
}