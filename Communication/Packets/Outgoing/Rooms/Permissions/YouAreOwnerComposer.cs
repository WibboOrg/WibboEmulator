namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;

internal sealed class YouAreOwnerComposer : ServerPacket
{
    public YouAreOwnerComposer()
        : base(ServerPacketHeader.ROOM_RIGHTS_OWNER)
    {

    }
}
