namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;

internal class SleepComposer : ServerPacket
{
    public SleepComposer(int virtualId, bool isSleeping)
        : base(ServerPacketHeader.UNIT_IDLE)
    {
        this.WriteInteger(virtualId);
        this.WriteBoolean(isSleeping);
    }
}
