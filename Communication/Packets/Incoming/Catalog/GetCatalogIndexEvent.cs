using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Game.Clients;
using Butterfly.Utilities;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetCatalogIndexEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            ServerPacketList packetList = new ServerPacketList();

            packetList.Add(new CatalogIndexComposer(Session, ButterflyEnvironment.GetGame().GetCatalog().GetPages()));//, Sub));
            packetList.Add(new CatalogItemDiscountComposer());
            packetList.Add(new BCBorrowedItemsComposer());

            Session.SendPacket(packetList);
        }
    }
}