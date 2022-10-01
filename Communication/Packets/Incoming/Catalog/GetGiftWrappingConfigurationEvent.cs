using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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