using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetStickyNoteEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.POSTIT)
            {
                return;
            }

            ServerPacket Response = new ServerPacket(ServerPacketHeader.FURNITURE_ITEMDATA);
            Response.WriteString(roomItem.Id.ToString());
            Response.WriteString(roomItem.ExtraData);
            Session.SendPacket(Response);
        }
    }
}
