namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal class AcceptBuddyEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser().Messenger == null)
        {
            return;
        }

        var count = packet.PopInt();
        for (var index = 0; index < count; ++index)
        {
            var num2 = packet.PopInt();
            var request = session.GetUser().Messenger.GetRequest(num2);
            if (request != null)
            {
                if (request.To != session.GetUser().Id)
                {
                    break;
                }

                if (!session.GetUser().Messenger.FriendshipExists(request.To))
                {
                    session.GetUser().Messenger.CreateFriendship(request.From);
                }

                session.GetUser().
                Messenger.HandleRequest(num2);
            }
        }
    }
}
