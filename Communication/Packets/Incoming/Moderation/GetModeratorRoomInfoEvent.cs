using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Game.Clients;
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

            RoomData data = ButterflyEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(RoomId);

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(data.Id);

            bool ownerInRoom = false;
            if (room != null && room.GetRoomUserManager().GetRoomUserByName(data.OwnerName) != null)
                ownerInRoom = true;

            Session.SendPacket(new ModeratorRoomInfoComposer(data, ownerInRoom));
        }
    }
}
