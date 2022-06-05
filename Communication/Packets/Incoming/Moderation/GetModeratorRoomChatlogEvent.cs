using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Game.Chat.Logs;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_mod"))
            {
                return;
            }

            Packet.PopInt(); //useless
            int roomID = Packet.PopInt();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(roomID);
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
