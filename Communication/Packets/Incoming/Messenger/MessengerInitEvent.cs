using Wibbo.Communication.Packets.Outgoing.Messenger;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class MessengerInitEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.GetUser().GetMessenger().OnStatusChanged();

            Session.SendPacket(new MessengerInitComposer());
            Session.SendPacket(new BuddyListComposer(Session.GetUser().GetMessenger().Friends));
            Session.GetUser().GetMessenger().ProcessOfflineMessages();
        }
    }
}
