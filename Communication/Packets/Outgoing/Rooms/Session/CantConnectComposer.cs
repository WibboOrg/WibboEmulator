namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.session;

internal class CantConnectComposer : ServerPacket
{
    public CantConnectComposer(int Error)
        : base(ServerPacketHeader.ROOM_ENTER_ERROR) => this.WriteInteger(Error);
}
