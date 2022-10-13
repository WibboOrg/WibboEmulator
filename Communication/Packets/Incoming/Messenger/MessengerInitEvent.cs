namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;

internal class MessengerInitEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null || session.GetUser().Messenger == null)
        {
            return;
        }

        session.GetUser().
        Messenger.OnStatusChanged();

        session.SendPacket(new MessengerInitComposer());
        session.SendPacket(new BuddyListComposer(session.GetUser().Messenger.Friends));
        session.GetUser().Messenger.ProcessOfflineMessages();
    }
}
