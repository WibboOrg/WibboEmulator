namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;

internal class TradeOfferMultipleItemsEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        var userTrade = room.GetUserTrade(session.GetUser().Id);
        if (userTrade == null)
        {
            return;
        }

        var itemCount = packet.PopInt();
        for (var i = 0; i < itemCount; i++)
        {
            var itemId = packet.PopInt();
            var userItem = session.GetUser().InventoryComponent.GetItem(itemId);
            if (userItem == null)
            {
                continue;
            }

            userTrade.OfferItem(session.GetUser().Id, userItem, false);
        }

        userTrade.UpdateTradeWindow();
    }
}
