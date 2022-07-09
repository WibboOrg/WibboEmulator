using Wibbo.Communication.Packets.Outgoing.MarketPlace;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Marketplace
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
