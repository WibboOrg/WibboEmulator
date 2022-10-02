using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class MessengerInitEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null || Session.GetUser().GetMessenger() == null)
                return;

            Session.GetUser().GetMessenger().OnStatusChanged();

            Session.SendPacket(new MessengerInitComposer());
            Session.SendPacket(new BuddyListComposer(Session.GetUser().GetMessenger().Friends));
            Session.GetUser().GetMessenger().ProcessOfflineMessages();
        }
    }
}
