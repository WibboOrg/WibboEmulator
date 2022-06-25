using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Rooms.Trading;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class TradingRemoveItemEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Trade userTrade = room.GetUserTrade(Session.GetUser().Id);
            Item userItem = Session.GetUser().GetInventoryComponent().GetItem(Packet.PopInt());
            if (userTrade == null || userItem == null)
            {
                return;
            }

            userTrade.TakeBackItem(Session.GetUser().Id, userItem);
        }
    }
}
