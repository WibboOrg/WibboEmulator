namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;

internal class SleepComposer : ServerPacket
{
    public SleepComposer(int VirtualId, bool IsSleeping)
        : base(ServerPacketHeader.UNIT_IDLE)
    {
        this.WriteInteger(VirtualId);
        this.WriteBoolean(IsSleeping);
    }
}
