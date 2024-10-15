namespace WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.Users;

internal sealed class OnGuideSessionStartedComposer : ServerPacket
{
    public OnGuideSessionStartedComposer(User Session, User requester)
        : base(ServerPacketHeader.GUIDE_SESSION_STARTED)
    {
        this.WriteInteger(requester.Id);
        this.WriteString(requester.Username);
        this.WriteString(requester.Look);
        this.WriteInteger(Session.Id);
        this.WriteString(Session.Username);
        this.WriteString(Session.Look);
    }
}
