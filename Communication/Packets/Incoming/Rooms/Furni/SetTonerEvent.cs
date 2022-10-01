using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class SetTonerEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true))
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.TONER)
            {
                return;
            }

            int num2 = Packet.PopInt();
            int num3 = Packet.PopInt();
            int num4 = Packet.PopInt();

            roomItem.ExtraData = "on," + num2 + "," + num3 + "," + num4;
            roomItem.UpdateState(true, true);
        }
    }
}