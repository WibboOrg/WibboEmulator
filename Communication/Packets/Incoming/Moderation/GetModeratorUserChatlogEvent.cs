using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasPermission("perm_chatlog"))
            {
                return;
            }

            int userId = Packet.PopInt();

            GameClient clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
            if (clientByUserId == null || clientByUserId.GetUser() == null)
            {
                List<ChatlogEntry> sortedMessages = new List<ChatlogEntry>();
                Session.SendPacket(new ModeratorUserChatlogComposer(userId, "User not online", Session.GetUser().CurrentRoomId, sortedMessages));
            }
            else
            {
                List<ChatlogEntry> sortedMessages = clientByUserId.GetUser().GetChatMessageManager().GetSortedMessages(0);

                Session.SendPacket(new ModeratorUserChatlogComposer(userId, clientByUserId.GetUser().Username, Session.GetUser().CurrentRoomId, sortedMessages));
            }
        }
    }
}
