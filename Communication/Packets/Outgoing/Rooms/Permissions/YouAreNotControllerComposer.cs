namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;

internal class YouAreNotControllerComposer : ServerPacket
{
    public YouAreNotControllerComposer()
        : base(ServerPacketHeader.ROOM_RIGHTS_CLEAR)
    {

    }
}
