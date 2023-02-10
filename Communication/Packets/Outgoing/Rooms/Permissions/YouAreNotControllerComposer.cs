namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;

internal sealed class YouAreNotControllerComposer : ServerPacket
{
    public YouAreNotControllerComposer()
        : base(ServerPacketHeader.ROOM_RIGHTS_CLEAR)
    {

    }
}
