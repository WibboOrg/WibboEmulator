using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using System.Drawing;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class InitializeFloorPlanSessionEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            ServerPacket Response = new ServerPacket(ServerPacketHeader.ROOM_MODEL_DOOR);
            Response.WriteInteger(room.GetGameMap().Model.DoorX); // x
            Response.WriteInteger(room.GetGameMap().Model.DoorY); // y
            Response.WriteInteger(room.GetGameMap().Model.DoorOrientation); // dir
            Session.SendPacket(Response);
        }
    }
}
