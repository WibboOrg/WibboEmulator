using Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class InitializeFloorPlanSessionEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Session.SendPacket(new FloorPlanSendDoorComposer(room.GetGameMap().Model.DoorX, room.GetGameMap().Model.DoorY, room.GetGameMap().Model.DoorOrientation));
        }
    }
}
