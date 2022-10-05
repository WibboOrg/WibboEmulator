namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.GameClients;

internal class GetCatalogPageEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var PageId = Packet.PopInt();
        var Something = Packet.PopInt();
        var CataMode = Packet.PopString();

        WibboEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out var Page);
        if (Page == null || Page.MinimumRank > session.GetUser().Rank)
        {
            return;
        }

        session.SendPacket(new CatalogPageComposer(Page, CataMode, session.Langue));
    }
}