using Butterfly.Game.Clients;
using Butterfly.Game.Moderation;
using Butterfly.Game.Rooms;

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

            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room room))

            Session.SendPacket(ModerationManager.SerializeRoomTool(ButterflyEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(RoomId)));
        }
    }
}
