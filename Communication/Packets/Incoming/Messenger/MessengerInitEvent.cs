namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;

internal sealed class MessengerInitEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null || Session.User.Messenger == null)
        {
            return;
        }

        Session.User.Messenger.OnStatusChanged();

        Session.SendPacket(new MessengerInitComposer());
        Session.SendPacket(new BuddyListComposer(Session.User.Messenger.Friends));
        Session.User.Messenger.ProcessOfflineMessages();
    }
}
