namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;

internal class RemoveBuddyEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser().GetMessenger() == null)
        {
            return;
        }

        var count = Packet.PopInt();

        if (count > 200)
        {
            count = 200;
        }

        int friendId;
        for (var index = 0; index < count; index++)
        {
            friendId = Packet.PopInt();
            session.GetUser().GetMessenger().DestroyFriendship(friendId);
        }
    }
}
