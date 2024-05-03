namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.GameClients;

internal sealed class GetCatalogOfferEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var id = packet.PopInt();

        var item = CatalogManager.FindItem(id, session.User);
        if (item == null)
        {
            return;
        }

        if (!CatalogManager.TryGetPage(item.PageID, out var page))
        {
            return;
        }

        if (!page.Enabled || !page.HavePermission(session.User))
        {
            return;
        }

        if (item.IsLimited)
        {
            return;
        }

        session.SendPacket(new CatalogOfferComposer(item, page.IsPremium));
    }
}
