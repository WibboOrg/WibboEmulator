namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal class DeclineBuddyEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser().GetMessenger() == null)
        {
            return;
        }

        var DeleteAllFriend = Packet.PopBoolean();
        var RequestCount = Packet.PopInt();

        if (!DeleteAllFriend && RequestCount == 1)
        {
            session.GetUser().GetMessenger().HandleRequest(Packet.PopInt());
        }
        else
        {
            session.GetUser().GetMessenger().HandleAllRequests();
        }
    }
}