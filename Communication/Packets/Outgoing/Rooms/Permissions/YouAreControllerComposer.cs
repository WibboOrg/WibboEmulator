namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;

internal class YouAreControllerComposer : ServerPacket
{
    public YouAreControllerComposer(int setting)
        : base(ServerPacketHeader.ROOM_RIGHTS) => this.WriteInteger(setting);
}
