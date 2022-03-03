using Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetOccupiedTilesEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Session.SendPacket(new FloorPlanFloorMapComposer(room.GetGameMap().CoordinatedItems));
        }
    }
}
