namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions
{
    internal class YouAreOwnerComposer : ServerPacket
    {
        public YouAreOwnerComposer()
            : base(ServerPacketHeader.ROOM_RIGHTS_OWNER)
        {

        }
    }
}
