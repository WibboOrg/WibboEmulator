using Wibbo.Communication.Packets.Outgoing.Moderation;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorRoomInfoEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_mod"))
            {
                return;
            }

            int RoomId = Packet.PopInt();

            RoomData data = WibboEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(RoomId);

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(data.Id);

            bool ownerInRoom = false;
            if (room != null && room.GetRoomUserManager().GetRoomUserByName(data.OwnerName) != null)
                ownerInRoom = true;

            Session.SendPacket(new ModeratorRoomInfoComposer(data, ownerInRoom));
        }
    }
}
