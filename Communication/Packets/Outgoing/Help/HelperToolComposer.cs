namespace WibboEmulator.Communication.Packets.Outgoing.Help;

internal class HelperToolComposer : ServerPacket
{
    public HelperToolComposer(bool onDuty, int count)
        : base(ServerPacketHeader.GUIDE_ON_DUTY_STATUS)
    {
        this.WriteBoolean(onDuty);
        this.WriteInteger(count);
        this.WriteInteger(0);
        this.WriteInteger(0);
    }
}
