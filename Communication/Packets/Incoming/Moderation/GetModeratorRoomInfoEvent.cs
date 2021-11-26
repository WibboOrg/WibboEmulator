using Butterfly.Game.Clients;
using Butterfly.Game.Moderation;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorRoomInfoEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int RoomId = Packet.PopInt();

            Session.SendPacket(ModerationManager.SerializeRoomTool(ButterflyEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(RoomId)));
        }
    }
}
