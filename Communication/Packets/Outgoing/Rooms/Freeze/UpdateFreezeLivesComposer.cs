namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Freeze;

internal class UpdateFreezeLivesComposer : ServerPacket
{
    public UpdateFreezeLivesComposer(int virtualId, int freezeLives)
        : base(ServerPacketHeader.UNIT_NUMBER)
    {
        this.WriteInteger(virtualId);
        this.WriteInteger(freezeLives);
    }
}
