namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;

internal class TradingConfirmEvent : IPacketEvent
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

        userTrade.CompleteTrade(session.GetUser().Id);
    }
}