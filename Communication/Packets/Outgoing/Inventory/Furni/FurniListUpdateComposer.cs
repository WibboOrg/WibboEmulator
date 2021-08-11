namespace Butterfly.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListUpdateComposer : ServerPacket
    {
        public FurniListUpdateComposer()
            : base(ServerPacketHeader.USER_FURNITURE_REFRESH)
        {

        }
    }
}
