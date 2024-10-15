namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;

internal sealed class MessengerInitEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null || session.User.Messenger == null)
        {
            return;
        }

        session.User.Messenger.OnStatusChanged();

        session.SendPacket(new MessengerInitComposer());
        session.SendPacket(new BuddyListComposer(session.User.Messenger.Friends));
        session.User.Messenger.ProcessOfflineMessages();
    }
}
