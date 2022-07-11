using WibboEmulator.Game.Users.Messenger;

namespace WibboEmulator.Communication.Packets.Outgoing.Messenger
{
    internal class BuddyRequestsComposer : ServerPacket
    {
        public BuddyRequestsComposer(Dictionary<int, MessengerRequest> requests)
            : base(ServerPacketHeader.MESSENGER_REQUESTS)
        {
            WriteInteger(requests.Count);
            WriteInteger(requests.Count);

            foreach (MessengerRequest request in requests.Values)
            {
                WriteInteger(request.From);
                WriteString(request.Username);
                WriteString("");
            }
        }
    }
}
