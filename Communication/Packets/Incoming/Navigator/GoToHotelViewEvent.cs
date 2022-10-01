using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
        }
    }
}