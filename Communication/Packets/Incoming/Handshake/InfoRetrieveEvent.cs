using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class InfoRetrieveEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new UserObjectComposer(Session.GetUser()));
            Session.SendPacket(new UserPerksComposer(Session.GetUser()));
        }
    }
}
