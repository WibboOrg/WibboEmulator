namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;

internal sealed class GetCreditsInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        Session.SendPacket(new CreditBalanceComposer(Session.User.Credits));
        Session.SendPacket(new ActivityPointsComposer(Session.User.WibboPoints, Session.User.LimitCoins));
    }
}
