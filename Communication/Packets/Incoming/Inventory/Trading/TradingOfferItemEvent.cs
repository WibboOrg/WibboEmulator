namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class TradingOfferItemEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        var userTrade = room.GetUserTrade(session.User.Id);
        var userItem = session.User.InventoryComponent.GetItem(packet.PopInt());
        if (userTrade == null || userItem == null)
        {
            return;
        }

        userTrade.OfferItem(session.User.Id, userItem);
    }
}
