namespace WibboEmulator.Communication.Packets.Incoming.Moderation;

using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Games.Chats.Logs;
using WibboEmulator.Games.GameClients;

internal sealed class GetModeratorUserChatlogEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("chatlog"))
        {
            return;
        }

        var userId = packet.PopInt();

        var clientByUserId = GameClientManager.GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.User == null)
        {
            var sortedMessages = new List<ChatlogEntry>();

            using var dbClient = DatabaseManager.Connection;
            var logChatList = LogChatDao.GetAllByUserId(dbClient, userId);
            if (logChatList.Count != 0)
            {
                foreach (var logChat in logChatList)
                {
                    sortedMessages.Add(new ChatlogEntry(logChat.UserId, logChat.UserName, logChat.RoomId, logChat.Type + logChat.Message, logChat.Timestamp));
                }
            }

            sortedMessages.Reverse();

            Session.SendPacket(new ModeratorUserChatlogComposer(userId, "User not online", Session.User.RoomId, sortedMessages));
        }
        else
        {
            var sortedMessages = clientByUserId.User.ChatMessageManager.GetSortedMessages(0);

            Session.SendPacket(new ModeratorUserChatlogComposer(userId, clientByUserId.User.Username, Session.User.RoomId, sortedMessages));
        }
    }
}
