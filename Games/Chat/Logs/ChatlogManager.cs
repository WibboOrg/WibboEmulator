namespace WibboEmulator.Games.Chat.Logs;
using System.Data;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;

public class ChatlogManager
{
    public int MessageCount => this.ListOfMessages.Count;

    public ChatlogManager() => this.ListOfMessages = new List<ChatlogEntry>();

    public void LoadUserChatlogs(IQueryAdapter dbClient, int userId)
    {
        var table = LogChatDao.GetAllByUserId(dbClient, userId);
        if (table == null)
        {
            return;
        }

        foreach (DataRow Row in table.Rows)
        {
            this.AddMessage(Convert.ToInt32(Row["user_id"]), Row["user_name"].ToString(), Convert.ToInt32(Row["room_id"]), Row["type"].ToString() + Row["message"].ToString(), (double)Row["timestamp"]);
        }
    }

    public void LoadRoomChatlogs(int roomId)
    {
        DataTable table;
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        table = LogChatDao.GetAllByRoomId(dbClient, roomId);
        if (table == null)
        {
            return;
        }

        foreach (DataRow Row in table.Rows)
        {
            this.AddMessage(Convert.ToInt32(Row["user_id"]), Row["user_name"].ToString(), Convert.ToInt32(Row["room_id"]), Row["type"].ToString() + Row["message"].ToString(), (double)Row["timestamp"]);
        }
    }

    public void AddMessage(int UserId, string Username, int RoomId, string MessageText, double timestamp)
    {
        var message = new ChatlogEntry(UserId, Username, RoomId, MessageText, timestamp);

        lock (this.ListOfMessages)
        {
            this.ListOfMessages.Add(message);
        }

        var CountMessage = this.ListOfMessages.Count;
        if (CountMessage >= 100)
        {
            this.ListOfMessages.RemoveRange(0, 1);
        }
    }

    public List<ChatlogEntry> GetSortedMessages(int roomId)
    {
        var list = new List<ChatlogEntry>();

        foreach (var chatMessage in this.ListOfMessages)
        {
            if (roomId == chatMessage.roomID || roomId == 0)
            {
                list.Add(chatMessage);
            }
        }

        list.Reverse();

        return list;
    }

    public List<ChatlogEntry> ListOfMessages { get; }
}
