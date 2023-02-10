namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;

internal sealed class FurniListUpdateComposer : ServerPacket
{
    public FurniListUpdateComposer()
        : base(ServerPacketHeader.USER_FURNITURE_REFRESH)
    {

    }
}
