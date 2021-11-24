using Butterfly.Game.Clients;
using Butterfly.Game.Moderation;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
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

            Session.SendPacket(ModerationManager.SerializeRoomChatlog(room));
        }
    }
}
