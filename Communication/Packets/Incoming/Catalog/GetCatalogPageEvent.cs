namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.GameClients;

internal class GetCatalogPageEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var pageId = packet.PopInt();

        _ = packet.PopInt();
        var cataMode = packet.PopString();

        _ = WibboEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out var page);
        if (page == null || page.MinimumRank > session.GetUser().Rank)
        {
            return;
        }

        session.SendPacket(new CatalogPageComposer(page, cataMode, session.Langue));
    }
}
