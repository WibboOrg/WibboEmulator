using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Game.Chat.Logs;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

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
            int roomID = Packet.PopInt();

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(roomID);
            if (room == null)
            {
                return;
            }

            List<ChatlogEntry> listReverse = new List<ChatlogEntry>();
            listReverse.AddRange(room.GetChatMessageManager().ListOfMessages);
            listReverse.Reverse();

            Session.SendPacket(new ModeratorRoomChatlogComposer(room, listReverse));
        }
    }
}
