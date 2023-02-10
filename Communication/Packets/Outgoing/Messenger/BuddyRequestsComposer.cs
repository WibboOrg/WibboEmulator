namespace WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.Users.Messenger;

internal sealed class BuddyRequestsComposer : ServerPacket
{
    public BuddyRequestsComposer(Dictionary<int, MessengerRequest> requests)
        : base(ServerPacketHeader.MESSENGER_REQUESTS)
    {
        this.WriteInteger(requests.Count);
        this.WriteInteger(requests.Count);

        foreach (var request in requests.Values)
        {
            this.WriteInteger(request.From);
            this.WriteString(request.Username);
            this.WriteString("");
        }
    }
}
