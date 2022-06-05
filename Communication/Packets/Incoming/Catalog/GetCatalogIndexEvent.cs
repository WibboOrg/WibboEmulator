using Wibbo.Communication.Packets.Outgoing.BuildersClub;
using Wibbo.Communication.Packets.Outgoing.Catalog;
using Wibbo.Game.Clients;
using Wibbo.Utilities;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetCatalogIndexEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            ServerPacketList packetList = new ServerPacketList();

            packetList.Add(new CatalogIndexComposer(Session, WibboEnvironment.GetGame().GetCatalog().GetPages()));//, Sub));
            packetList.Add(new CatalogItemDiscountComposer());
            packetList.Add(new BCBorrowedItemsComposer());

            Session.SendPacket(packetList);
        }
    }
}