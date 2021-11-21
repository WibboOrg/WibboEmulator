using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SetMannequinFigureEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MANNEQUIN)
            {
                return;
            }

            room.SendPacket(new ObjectUpdateComposer(roomItem, room.RoomData.OwnerId));
        }
    }
}
