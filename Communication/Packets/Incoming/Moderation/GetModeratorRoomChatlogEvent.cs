using Wibbo.Communication.Packets.Outgoing.Moderation;
using Wibbo.Game.Chat.Logs;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
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
