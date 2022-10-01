using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasPermission("perm_mod"))
            {
                return;
            }

            Packet.PopInt(); //useless
            int roomId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out Room room))
                return;

            List<ChatlogEntry> listReverse = new List<ChatlogEntry>();
            listReverse.AddRange(room.GetChatMessageManager().ListOfMessages);
            listReverse.Reverse();

            Session.SendPacket(new ModeratorRoomChatlogComposer(room, listReverse));
        }
    }
}
