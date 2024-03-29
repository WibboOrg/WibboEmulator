namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class FlatCreatedComposer : ServerPacket
{
    public FlatCreatedComposer(int roomID, string roomName)
        : base(ServerPacketHeader.ROOM_CREATED)
    {
        this.WriteInteger(roomID);
        this.WriteString(roomName);
    }
}
