using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class SetTonerEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
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