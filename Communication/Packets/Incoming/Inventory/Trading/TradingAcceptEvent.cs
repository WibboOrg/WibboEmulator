namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;

internal class TradingAcceptEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
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

        userTrade.Accept(session.GetUser().Id);
    }
}