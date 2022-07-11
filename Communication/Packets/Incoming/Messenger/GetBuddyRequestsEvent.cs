using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetBuddyRequestsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new BuddyRequestsComposer(Session.GetUser().GetMessenger().Requests));
        }
    }
}
