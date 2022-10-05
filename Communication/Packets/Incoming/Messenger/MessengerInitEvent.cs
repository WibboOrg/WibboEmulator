namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;

internal class MessengerInitEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null || session.GetUser().GetMessenger() == null)
        {
            return;
        }

        session.GetUser().GetMessenger().OnStatusChanged();

        session.SendPacket(new MessengerInitComposer());
        session.SendPacket(new BuddyListComposer(session.GetUser().GetMessenger().Friends));
        session.GetUser().GetMessenger().ProcessOfflineMessages();
    }
}
