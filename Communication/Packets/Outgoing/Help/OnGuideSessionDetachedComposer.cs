namespace WibboEmulator.Communication.Packets.Outgoing.Help;

internal sealed class OnGuideSessionDetachedComposer : ServerPacket
{
    public OnGuideSessionDetachedComposer()
        : base(ServerPacketHeader.GUIDE_SESSION_DETACHED)
    {

    }
}
