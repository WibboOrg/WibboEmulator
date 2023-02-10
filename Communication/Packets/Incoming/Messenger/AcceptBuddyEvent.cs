namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal sealed class AcceptBuddyEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User.Messenger == null)
        {
            return;
        }

        var count = packet.PopInt();
        for (var index = 0; index < count; ++index)
        {
            var num2 = packet.PopInt();
            var request = session.User.Messenger.GetRequest(num2);
            if (request != null)
            {
                if (request.To != session.User.Id)
                {
                    break;
                }

                if (!session.User.Messenger.FriendshipExists(request.To))
                {
                    session.User.Messenger.CreateFriendship(request.From);
                }

                session.User.Messenger.HandleRequest(num2);
            }
        }
    }
}
