namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal class DeclineBuddyEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser().GetMessenger() == null)
        {
            return;
        }

        var DeleteAllFriend = packet.PopBoolean();
        var RequestCount = packet.PopInt();

        if (!DeleteAllFriend && RequestCount == 1)
        {
            session.GetUser().GetMessenger().HandleRequest(packet.PopInt());
        }
        else
        {
            session.GetUser().GetMessenger().HandleAllRequests();
        }
    }
}