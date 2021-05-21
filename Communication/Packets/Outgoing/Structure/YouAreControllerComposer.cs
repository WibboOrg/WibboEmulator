namespace Butterfly.Communication.Packets.Outgoing.Structure
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
