using WibboEmulator.Communication.Packets.Outgoing.Rooms.FloorPlan;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
