namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal sealed class FurnitureAliasesComposer : ServerPacket
{
    public FurnitureAliasesComposer()
        : base(ServerPacketHeader.FURNITURE_ALIASES) => this.WriteInteger(0);
}
