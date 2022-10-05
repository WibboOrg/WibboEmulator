namespace WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.Users;

internal class OnGuideSessionStartedComposer : ServerPacket
{
    public OnGuideSessionStartedComposer(User session, User requester)
        : base(ServerPacketHeader.GUIDE_SESSION_STARTED)
    {
        this.WriteInteger(requester.Id);
        this.WriteString(requester.Username);
        this.WriteString(requester.Look);
        this.WriteInteger(session.Id);
        this.WriteString(session.Username);
        this.WriteString(session.Look);
    }
}
