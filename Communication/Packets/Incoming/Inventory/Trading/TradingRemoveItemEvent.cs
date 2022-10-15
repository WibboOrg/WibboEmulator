namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;

internal class TradingRemoveItemEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var userTrade = room.GetUserTrade(session.User.Id);
        var userItem = session.User.InventoryComponent.GetItem(packet.PopInt());
        if (userTrade == null || userItem == null)
        {
            return;
        }

        userTrade.TakeBackItem(session.User.Id, userItem);
    }
}
