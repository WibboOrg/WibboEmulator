namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Games.GameClients;

internal sealed class GetMarketplaceCanMakeOfferEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var errorCode = 1;

        Session.SendPacket(new MarketplaceCanMakeOfferResultComposer(errorCode));
    }
}
