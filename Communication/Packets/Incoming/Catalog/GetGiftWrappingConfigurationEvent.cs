using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetGiftWrappingConfigurationEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new GiftWrappingConfigurationComposer());
        }
    }
}