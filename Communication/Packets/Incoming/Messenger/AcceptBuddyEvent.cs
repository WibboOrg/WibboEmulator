namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;

internal class AcceptBuddyEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser().GetMessenger() == null)
        {
            return;
        }

        var Count = Packet.PopInt();
        for (var index = 0; index < Count; ++index)
        {
            var num2 = Packet.PopInt();
            var request = session.GetUser().GetMessenger().GetRequest(num2);
            if (request != null)
            {
                if (request.To != session.GetUser().Id)
                {
                    break;
                }

                if (!session.GetUser().GetMessenger().FriendshipExists(request.To))
                {
                    session.GetUser().GetMessenger().CreateFriendship(request.From);
                }

                session.GetUser().GetMessenger().HandleRequest(num2);
            }
        }
    }
}