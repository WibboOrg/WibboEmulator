namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;

internal sealed class CarryObjectComposer : ServerPacket
{
    public CarryObjectComposer(int virtualID, int itemID)
        : base(ServerPacketHeader.UNIT_HAND_ITEM)
    {
        this.WriteInteger(virtualID);
        this.WriteInteger(itemID);
    }
}
