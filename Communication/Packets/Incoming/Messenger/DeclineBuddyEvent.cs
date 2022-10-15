namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal class DeclineBuddyEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User.Messenger == null)
        {
            return;
        }

        var deleteAllFriend = packet.PopBoolean();
        var requestCount = packet.PopInt();

        if (!deleteAllFriend && requestCount == 1)
        {
            session.User.Messenger.HandleRequest(packet.PopInt());
        }
        else
        {
            session.User.Messenger.HandleAllRequests();
        }
    }
}
