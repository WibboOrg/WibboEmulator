using Butterfly.Communication.Packets.Outgoing.MarketPlace;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class GetOwnOffersEvent : IPacketEvent
    {
        public void Parse(Game.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new MarketPlaceOwnOffersComposer(Session.GetHabbo().Id));
        }
    }
}
