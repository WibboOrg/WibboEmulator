namespace WibboEmulator.Games.Chats.Logs;
using System.Data;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Interfaces;

public class ChatlogManager
{
    public ChatlogManager() => this.ListOfMessages = new List<ChatlogEntry>();

    public void LoadUserChatlogs(IQueryAdapter dbClient, int userId)
    {
        var table = LogChatDao.GetAllByUserId(dbClient, userId);
        if (table == null)
        {
            return;
        }

        foreach (DataRow row in table.Rows)
        {
            this.AddMessage(Convert.ToInt32(row["user_id"]), row["user_name"].ToString(), Convert.ToInt32(row["room_id"]), row["type"].ToString() + row["message"].ToString(), (int)row["timestamp"]);
        }
    }

    public void LoadRoomChatlogs(int roomId, IQueryAdapter dbClient)
    {
        var logs = LogChatDao.GetAllByRoomId(dbClient, roomId);
        if (logs == null)
        {
            return;
        }

        foreach (DataRow row in logs.Rows)
        {
            this.AddMessage(Convert.ToInt32(row["user_id"]), row["user_name"].ToString(), Convert.ToInt32(row["room_id"]), row["type"].ToString() + row["message"].ToString(), (int)row["timestamp"]);
        }
    }

    public void AddMessage(int userId, string username, int roomId, string messageText, double timestamp)
    {
        var message = new ChatlogEntry(userId, username, roomId, messageText, timestamp);

        lock (this.ListOfMessages)
        {
            this.ListOfMessages.Add(message);
        }

        var countMessage = this.ListOfMessages.Count;
        if (countMessage >= 100)
        {
            this.ListOfMessages.RemoveRange(0, 1);
        }
    }

    public List<ChatlogEntry> GetSortedMessages(int roomId)
    {
        var list = new List<ChatlogEntry>();

        foreach (var chatMessage in this.ListOfMessages)
        {
            if (roomId == chatMessage.RoomID || roomId == 0)
            {
                list.Add(chatMessage);
            }
        }

        list.Reverse();

        return list;
    }

    public List<ChatlogEntry> ListOfMessages { get; }
}
