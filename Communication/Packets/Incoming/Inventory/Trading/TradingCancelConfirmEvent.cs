namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;

internal class TradingCancelConfirmEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        room.TryStopTrade(session.GetUser().Id);
    }
}
