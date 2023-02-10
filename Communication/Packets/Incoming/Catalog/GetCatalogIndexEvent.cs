namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.BuildersClub;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal sealed class GetCatalogIndexEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var packetList = new ServerPacketList();

        packetList.Add(new CatalogIndexComposer(session, WibboEnvironment.GetGame().GetCatalog().GetPages()));//, Sub));
        packetList.Add(new CatalogItemDiscountComposer());
        packetList.Add(new BCBorrowedItemsComposer());

        session.SendPacket(packetList);
    }
}