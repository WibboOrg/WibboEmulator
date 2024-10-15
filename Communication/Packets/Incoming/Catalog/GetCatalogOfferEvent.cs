namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.GameClients;

internal sealed class GetCatalogOfferEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var id = packet.PopInt();

        var item = CatalogManager.FindItem(id, Session.User);
        if (item == null)
        {
            return;
        }

        if (!CatalogManager.TryGetPage(item.PageID, out var page))
        {
            return;
        }

        if (!page.Enabled || !page.HavePermission(Session.User))
        {
            return;
        }

        if (item.IsLimited)
        {
            return;
        }

        Session.SendPacket(new CatalogOfferComposer(item, page.IsPremium));
    }
}
