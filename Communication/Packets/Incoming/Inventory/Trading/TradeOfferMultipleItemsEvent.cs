namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class TradeOfferMultipleItemsEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        var userTrade = room.GetUserTrade(session.User.Id);
        if (userTrade == null)
        {
            return;
        }

        var itemCount = packet.PopInt();
        for (var i = 0; i < itemCount; i++)
        {
            var itemId = packet.PopInt();
            var userItem = session.User.InventoryComponent.GetItem(itemId);
            if (userItem == null)
            {
                continue;
            }

            userTrade.OfferItem(session.User.Id, userItem, false);
        }

        userTrade.UpdateTradeWindow();
    }
}
