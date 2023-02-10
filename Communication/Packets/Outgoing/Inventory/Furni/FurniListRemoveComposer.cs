namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;

internal sealed class FurniListRemoveComposer : ServerPacket
{
    public FurniListRemoveComposer(int id)
        : base(ServerPacketHeader.USER_FURNITURE_REMOVE) => this.WriteInteger(id);
}
