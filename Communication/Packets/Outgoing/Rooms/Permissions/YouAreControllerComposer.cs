namespace Wibbo.Communication.Packets.Outgoing.Rooms.Permissions
{
    internal class YouAreControllerComposer : ServerPacket
    {
        public YouAreControllerComposer(int Setting)
            : base(ServerPacketHeader.ROOM_RIGHTS)
        {
            this.WriteInteger(Setting);
        }
    }
}
