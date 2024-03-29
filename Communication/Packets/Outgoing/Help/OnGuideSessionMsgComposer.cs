namespace WibboEmulator.Communication.Packets.Outgoing.Help;

internal sealed class OnGuideSessionMsgComposer : ServerPacket
{
    public OnGuideSessionMsgComposer(string message, int userId)
        : base(ServerPacketHeader.GUIDE_SESSION_MESSAGE)
    {
        this.WriteString(message);
        this.WriteInteger(userId);
    }
}
