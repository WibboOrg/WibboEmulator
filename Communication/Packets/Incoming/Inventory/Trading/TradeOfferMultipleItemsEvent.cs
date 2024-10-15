namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class TradeOfferMultipleItemsEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var userTrade = room.GetUserTrade(Session.User.Id);
        if (userTrade == null)
        {
            return;
        }

        var itemCount = packet.PopInt();
        for (var i = 0; i < itemCount; i++)
        {
            var itemId = packet.PopInt();
            var userItem = Session.User.InventoryComponent.GetItem(itemId);
            if (userItem == null)
            {
                continue;
            }

            userTrade.OfferItem(Session.User.Id, userItem, false);
        }

        userTrade.UpdateTradeWindow();
    }
}
