namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal sealed class DeclineBuddyEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User.Messenger == null)
        {
            return;
        }

        var deleteAllFriend = packet.PopBoolean();
        var requestCount = packet.PopInt();

        if (!deleteAllFriend && requestCount == 1)
        {
            Session.User.Messenger.HandleRequest(packet.PopInt());
        }
        else
        {
            Session.User.Messenger.HandleAllRequests();
        }
    }
}
