using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetWardrobeEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new WardrobeComposer(Session.GetUser().GetWardrobeComponent().GetWardrobes()));
        }
    }
}