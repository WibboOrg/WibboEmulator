using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Marketplace
{
    internal class GetOwnOffersEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new MarketPlaceOwnOffersComposer(Session.GetUser().Id));
        }
    }
}
