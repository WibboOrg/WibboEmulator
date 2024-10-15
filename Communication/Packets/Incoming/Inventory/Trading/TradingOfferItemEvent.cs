namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class TradingOfferItemEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var userTrade = room.GetUserTrade(Session.User.Id);
        var userItem = Session.User.InventoryComponent.GetItem(packet.PopInt());
        if (userTrade == null || userItem == null)
        {
            return;
        }

        userTrade.OfferItem(Session.User.Id, userItem);
    }
}
