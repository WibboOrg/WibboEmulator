namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FurniListUpdateComposer : ServerPacket
    {
        public FurniListUpdateComposer()
            : base(ServerPacketHeader.USER_FURNITURE_REFRESH)
        {

        }
    }
}
