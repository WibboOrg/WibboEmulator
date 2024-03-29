namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;

internal sealed class FlatAccessibleComposer : ServerPacket
{
    public FlatAccessibleComposer(string username)
        : base(ServerPacketHeader.ROOM_DOORBELL_ACCEPTED) => this.WriteString(username);
}
