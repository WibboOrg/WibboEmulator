namespace WibboEmulator.Communication.Packets.Outgoing.Help;

internal class OnGuideSessionAttachedComposer : ServerPacket
{
    public OnGuideSessionAttachedComposer(bool enable, int userId, string mesage, int time)
        : base(ServerPacketHeader.GUIDE_SESSION_ATTACHED)
    {
        this.WriteBoolean(enable);
        this.WriteInteger(userId);
        this.WriteString(mesage);
        this.WriteInteger(time);
    }
}
