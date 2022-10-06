namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

internal class FloodControlComposer : ServerPacket
{
    public FloodControlComposer(int floodTime)
        : base(ServerPacketHeader.FLOOD_CONTROL) => this.WriteInteger(floodTime);
}
