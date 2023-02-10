namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;

internal sealed class DanceComposer : ServerPacket
{
    public DanceComposer(int virtualId, int dance)
        : base(ServerPacketHeader.UNIT_DANCE)
    {
        this.WriteInteger(virtualId);
        this.WriteInteger(dance);
    }
}
