using Wibbo.Game.Users.Messenger;

namespace Wibbo.Communication.Packets.Outgoing.Messenger
{
    internal class NewBuddyRequestComposer : ServerPacket
    {
        public NewBuddyRequestComposer(MessengerRequest request)
            : base(ServerPacketHeader.MESSENGER_REQUEST)
        {
            WriteInteger(request.From);
            WriteString(request.Username);
            WriteString("");
        }
    }
}
