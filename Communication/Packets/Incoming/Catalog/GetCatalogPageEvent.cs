namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.GameClients;

internal sealed class GetCatalogPageEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var pageId = packet.PopInt();

        var offerId = packet.PopInt();
        var cataMode = packet.PopString();

        _ = CatalogManager.TryGetPage(pageId, out var page);
        if (page == null || !page.HavePermission(Session.User))
        {
            return;
        }

        if (page.Template == "club_gifts")
        {
            Session.SendPacket(new ClubGiftInfoComposer([.. page.Items.Values]));
        }

        Session.SendPacket(new CatalogPageComposer(page, cataMode, Session.Language, offerId));
    }
}
