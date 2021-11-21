using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class GetOwnOffersEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new MarketPlaceOwnOffersComposer(Session.GetHabbo().Id));
        }
    }
}
