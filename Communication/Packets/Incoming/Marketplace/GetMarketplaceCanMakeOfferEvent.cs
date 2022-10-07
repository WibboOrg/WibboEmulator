namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Games.GameClients;

internal class GetMarketplaceCanMakeOfferEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var errorCode = 1;

        session.SendPacket(new MarketplaceCanMakeOfferResultComposer(errorCode));
    }
}
