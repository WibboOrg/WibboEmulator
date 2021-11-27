using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionStartedComposer : ServerPacket
    {
        public OnGuideSessionStartedComposer(User session, User requester)
            : base(ServerPacketHeader.GUIDE_SESSION_STARTED)
        {
            WriteInteger(requester.Id);
            WriteString(requester.Username);
            WriteString(requester.Look);
            WriteInteger(session.Id);
            WriteString(session.Username);
            WriteString(session.Look);
        }
    }
}
