using Wibbo.Communication.Packets.Outgoing.Moderation;
using Wibbo.Game.Chat.Logs;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            int userId = Packet.PopInt();

            Client clientByUserId = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
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
