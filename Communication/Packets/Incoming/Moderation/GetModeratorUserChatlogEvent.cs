namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chats.Logs;
using WibboEmulator.Games.GameClients;

internal class GetModeratorUserChatlogEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("perm_chatlog"))
        {
            return;
        }

        var userId = packet.PopInt();

        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.User == null)
        {
            var sortedMessages = new List<ChatlogEntry>();
            session.SendPacket(new ModeratorUserChatlogComposer(userId, "User not online", session.User.CurrentRoomId, sortedMessages));
        }
        else
        {
            var sortedMessages = clientByUserId.User.ChatMessageManager.GetSortedMessages(0);

            session.SendPacket(new ModeratorUserChatlogComposer(userId, clientByUserId.User.Username, session.User.CurrentRoomId, sortedMessages));
        }
    }
}
