namespace WibboEmulator.Communication.Packets.Incoming.Moderation;

using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Games.Chats.Logs;
using WibboEmulator.Games.GameClients;

internal sealed class GetModeratorUserChatlogEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("chatlog"))
        {
            return;
        }

        var userId = packet.PopInt();

        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.User == null)
        {
            var sortedMessages = new List<ChatlogEntry>();

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            var table = LogChatDao.GetAllByUserId(dbClient, userId);
            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    sortedMessages.Add(new ChatlogEntry(Convert.ToInt32(row["user_id"]), row["user_name"].ToString(), Convert.ToInt32(row["room_id"]), row["type"].ToString() + row["message"].ToString(), (int)row["timestamp"]));
                }
            }

            sortedMessages.Reverse();

            session.SendPacket(new ModeratorUserChatlogComposer(userId, "User not online", session.User.CurrentRoomId, sortedMessages));
        }
        else
        {
            var sortedMessages = clientByUserId.User.ChatMessageManager.GetSortedMessages(0);

            session.SendPacket(new ModeratorUserChatlogComposer(userId, clientByUserId.User.Username, session.User.CurrentRoomId, sortedMessages));
        }
    }
}
