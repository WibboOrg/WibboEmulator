using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorRoomInfoEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasPermission("perm_mod"))
            {
                return;
            }

            int roomId = Packet.PopInt();

            RoomData data = WibboEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(roomId);

            bool ownerInRoom = false;
            if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(data.Id, out Room room))
            {
                if(room.GetRoomUserManager().GetRoomUserByName(data.OwnerName) != null)
                    ownerInRoom = true;
            }

            Session.SendPacket(new ModeratorRoomInfoComposer(data, ownerInRoom));
        }
    }
}
