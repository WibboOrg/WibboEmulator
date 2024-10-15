namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.BuildersClub;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal sealed class GetCatalogIndexEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var packetList = new ServerPacketList();

        packetList.Add(new CatalogIndexComposer(Session, CatalogManager.Pages));//, Sub));
        packetList.Add(new CatalogItemDiscountComposer());
        packetList.Add(new BCBorrowedItemsComposer());

        Session.SendPacket(packetList);
    }
}
