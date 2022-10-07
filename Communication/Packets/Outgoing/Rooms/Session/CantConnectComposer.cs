namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;

internal class CantConnectComposer : ServerPacket
{
    public CantConnectComposer(int error)
        : base(ServerPacketHeader.ROOM_ENTER_ERROR) => this.WriteInteger(error);
}
