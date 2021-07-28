using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Support;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorRoomInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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
