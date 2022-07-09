using Wibbo.Communication.Packets.Outgoing.Rooms.Session;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GoToHotelViewEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new CloseConnectionComposer());
            Session.GetUser().LoadingRoomId = 0;

            if (Session.GetUser() == null || !Session.GetUser().InRoom)
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room != null)
            {
                room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
            }
        }
    }
}