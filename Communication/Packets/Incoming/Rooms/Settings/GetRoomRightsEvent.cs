using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
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
