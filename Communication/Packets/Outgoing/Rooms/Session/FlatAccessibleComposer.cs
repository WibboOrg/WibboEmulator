namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.session;

internal class FlatAccessibleComposer : ServerPacket
{
    public FlatAccessibleComposer(string username)
        : base(ServerPacketHeader.ROOM_DOORBELL_ACCEPTED) => this.WriteString(username);
}
