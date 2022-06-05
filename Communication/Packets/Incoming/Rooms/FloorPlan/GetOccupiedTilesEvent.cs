using Wibbo.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetOccupiedTilesEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Session.SendPacket(new FloorPlanFloorMapComposer(room.GetGameMap().CoordinatedItems));
        }
    }
}
