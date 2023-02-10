namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;

internal sealed class ActionComposer : ServerPacket
{
    public ActionComposer(int virtualId, int actionId)
        : base(ServerPacketHeader.UNIT_EXPRESSION)
    {
        this.WriteInteger(virtualId);
        this.WriteInteger(actionId);
    }
}
