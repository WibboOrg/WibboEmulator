namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.GameClients;

internal class GetModeratorUserChatlogEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().HasPermission("perm_chatlog"))
        {
            return;
        }

        var userId = Packet.PopInt();

        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.GetUser() == null)
        {
            var sortedMessages = new List<ChatlogEntry>();
            session.SendPacket(new ModeratorUserChatlogComposer(userId, "User not online", session.GetUser().CurrentRoomId, sortedMessages));
        }
        else
        {
            var sortedMessages = clientByUserId.GetUser().GetChatMessageManager().GetSortedMessages(0);

            session.SendPacket(new ModeratorUserChatlogComposer(userId, clientByUserId.GetUser().Username, session.GetUser().CurrentRoomId, sortedMessages));
        }
    }
}
