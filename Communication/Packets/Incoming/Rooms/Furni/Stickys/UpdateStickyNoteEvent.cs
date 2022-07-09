using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class UpdateStickyNoteEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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
