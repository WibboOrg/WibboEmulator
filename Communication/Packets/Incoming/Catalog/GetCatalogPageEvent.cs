namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.GameClients;

internal sealed class GetCatalogPageEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var pageId = packet.PopInt();

        var offerId = packet.PopInt();
        var cataMode = packet.PopString();

        _ = CatalogManager.TryGetPage(pageId, out var page);
        if (page == null || !page.HavePermission(session.User))
        {
            return;
        }

        if (page.Template == "club_gifts")
        {
            session.SendPacket(new ClubGiftInfoComposer(page.Items.Values.ToList()));
        }

        session.SendPacket(new CatalogPageComposer(page, cataMode, session.Language, offerId));
    }
}
