using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UpdateStickyNoteEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.POSTIT)
            {
                return;
            }

            string Color = Packet.PopString();
            string Message = Packet.PopString();

            if (!room.CheckRights(Session) && !Message.StartsWith(roomItem.ExtraData))
            {
                return;
            }

            switch (Color)
            {
                case "FFFF33":
                case "FF9CFF":
                case "9CCEFF":
                case "9CFF9C":
                    roomItem.ExtraData = Color + " " + Message;
                    roomItem.UpdateState(true, true);
                    break;
            }

        }
    }
}
