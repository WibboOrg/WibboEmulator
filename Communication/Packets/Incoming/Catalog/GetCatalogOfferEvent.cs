namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.GameClients;

internal class GetCatalogOfferEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var id = Packet.PopInt();
        var Item = WibboEnvironment.GetGame().GetCatalog().FindItem(id, session.GetUser().Rank);
        if (Item == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(Item.PageID, out var Page))
        {
            return;
        }

        if (!Page.Enabled || Page.MinimumRank > session.GetUser().Rank)
        {
            return;
        }

        if (Item.IsLimited)
        {
            return;
        }

        session.SendPacket(new CatalogOfferComposer(Item));
    }
}