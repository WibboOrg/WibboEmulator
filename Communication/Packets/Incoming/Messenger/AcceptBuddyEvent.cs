namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal sealed class AcceptBuddyEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User.Messenger == null)
        {
            return;
        }

        var count = packet.PopInt();
        for (var index = 0; index < count; ++index)
        {
            var num2 = packet.PopInt();
            var request = Session.User.Messenger.GetRequest(num2);
            if (request != null)
            {
                if (request.To != Session.User.Id)
                {
                    break;
                }

                if (!Session.User.Messenger.FriendshipExists(request.To))
                {
                    Session.User.Messenger.CreateFriendship(request.From);
                }

                Session.User.Messenger.HandleRequest(num2);
            }
        }
    }
}
