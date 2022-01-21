using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using System.Drawing;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetOccupiedTilesEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            ServerPacket Response = new ServerPacket(ServerPacketHeader.ROOM_MODEL_BLOCKED_TILES);
            Response.WriteInteger(room.GetGameMap().CoordinatedItems.Count); //nombre de case

            foreach (Point Coords in room.GetGameMap().CoordinatedItems.Keys)
            {
                Response.WriteInteger(Coords.X); // x
                Response.WriteInteger(Coords.Y); // y
            }

            Session.SendPacket(Response);
        }
    }
}
