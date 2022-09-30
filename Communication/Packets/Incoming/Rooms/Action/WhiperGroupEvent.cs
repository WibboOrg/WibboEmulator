using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class WhiperGroupEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            string name = Packet.PopString();

            RoomUser roomUserByUserTarget = room.GetRoomUserManager().GetRoomUserByName(name);
            if (roomUserByUserTarget == null)
            {
                return;
            }

            if (!roomUserByUserId.WhiperGroupUsers.Contains(roomUserByUserTarget.GetUsername()))
            {
                if (roomUserByUserId.WhiperGroupUsers.Count >= 5)
                {
                    return;
                }

                roomUserByUserId.WhiperGroupUsers.Add(roomUserByUserTarget.GetUsername());
            }
            else
            {
                roomUserByUserId.WhiperGroupUsers.Remove(roomUserByUserTarget.GetUsername());
            }
        }
    }
}