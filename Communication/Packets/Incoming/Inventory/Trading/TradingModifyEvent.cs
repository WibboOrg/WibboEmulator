namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;

internal class TradingModifyEvent : IPacketEvent
{
    public double Delay => 0;

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

        userTrade.Unaccept(session.GetUser().Id);
    }
}
