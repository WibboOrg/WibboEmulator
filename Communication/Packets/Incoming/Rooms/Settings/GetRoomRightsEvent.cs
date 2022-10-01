using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetRoomRightsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            Room Instance = Session.GetUser().CurrentRoom;
            if (Instance == null)
            {
                return;
            }

            if (!Instance.CheckRights(Session))
            {
                return;
            }

            Session.SendPacket(new RoomRightsListComposer(Instance));
        }
    }
}
