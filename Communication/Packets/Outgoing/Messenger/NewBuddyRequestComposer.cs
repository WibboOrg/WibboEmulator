namespace WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.Users.Messenger;

internal class NewBuddyRequestComposer : ServerPacket
{
    public NewBuddyRequestComposer(MessengerRequest request)
        : base(ServerPacketHeader.MESSENGER_REQUEST)
    {
        this.WriteInteger(request.From);
        this.WriteString(request.Username);
        this.WriteString("");
    }
}
