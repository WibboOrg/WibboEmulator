namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;

internal sealed class GetCreditsInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        session.SendPacket(new CreditBalanceComposer(session.User.Credits));
        session.SendPacket(new ActivityPointsComposer(session.User.WibboPoints, session.User.LimitCoins));
    }
}
