using Butterfly.Game.User;

namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionStartedComposer : ServerPacket
    {
        public OnGuideSessionStartedComposer(Habbo session, Habbo requester)
            : base(ServerPacketHeader.OnGuideSessionStarted)
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
