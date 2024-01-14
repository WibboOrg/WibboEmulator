namespace WibboEmulator.Games.Chats.Logs;
using System.Data;
using WibboEmulator.Database.Daos.Log;

public class ChatlogManager
{
    public ChatlogManager() => this.ListOfMessages = new List<ChatlogEntry>();

    public void LoadUserChatlogs(IDbConnection dbClient, int userId)
    {
        var logChatList = LogChatDao.GetAllByUserId(dbClient, userId);
        if (logChatList.Count == 0)
        {
            return;
        }

        foreach (var logChat in logChatList)
        {
            this.AddMessage(logChat.UserId, logChat.UserName, logChat.RoomId, logChat.Type + logChat.Message, logChat.Timestamp);
        }
    }

    public void LoadRoomChatlogs(int roomId, IDbConnection dbClient)
    {
        var logChatList = LogChatDao.GetAllByRoomId(dbClient, roomId);
        if (logChatList.Count == 0)
        {
            return;
        }

        foreach (var logChat in logChatList)
        {
            this.AddMessage(logChat.UserId, logChat.UserName, logChat.RoomId, logChat.Type + logChat.Message, logChat.Timestamp);
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
