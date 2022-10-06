namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;

internal class GetCreditsInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        session.SendPacket(new CreditBalanceComposer(session.GetUser().Credits));
        session.SendPacket(new ActivityPointsComposer(session.GetUser().WibboPoints, session.GetUser().LimitCoins));
    }
}
