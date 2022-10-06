namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.GameClients;

internal class GetCatalogPageEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var PageId = packet.PopInt();
        var Something = packet.PopInt();
        var CataMode = packet.PopString();

        _ = WibboEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out var Page);
        if (Page == null || Page.MinimumRank > session.GetUser().Rank)
        {
            return;
        }

        session.SendPacket(new CatalogPageComposer(Page, CataMode, session.Langue));
    }
}