using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class WhiperGroupEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

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