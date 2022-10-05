namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Freeze;

internal class UpdateFreezeLivesComposer : ServerPacket
{
    public UpdateFreezeLivesComposer(int VirtualId, int FreezeLives)
        : base(ServerPacketHeader.UNIT_NUMBER)
    {
        this.WriteInteger(VirtualId);
        this.WriteInteger(FreezeLives);
    }
}
