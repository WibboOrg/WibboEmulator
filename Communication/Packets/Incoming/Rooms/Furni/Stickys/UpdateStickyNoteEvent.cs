using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class UpdateStickyNoteEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session))
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
