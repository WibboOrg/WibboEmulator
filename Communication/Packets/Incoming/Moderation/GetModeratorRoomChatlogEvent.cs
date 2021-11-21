using Butterfly.Game.Clients;
using Butterfly.Game.Moderation;

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

            Packet.PopInt();
            int roomID = Packet.PopInt();

            if (ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(roomID) == null)
            {
                return;
            }

            Session.SendPacket(ModerationManager.SerializeRoomChatlog(roomID));
        }
    }
}
