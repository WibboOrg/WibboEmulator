namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;

internal class TradingCancelEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || !session.User.InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        room.TryStopTrade(session.User.Id);
    }
}
