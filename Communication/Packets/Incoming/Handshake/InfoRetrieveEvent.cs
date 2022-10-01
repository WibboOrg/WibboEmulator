using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class InfoRetrieveEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new UserObjectComposer(Session.GetUser()));
            Session.SendPacket(new UserPerksComposer(Session.GetUser()));
        }
    }
}
