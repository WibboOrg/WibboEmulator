namespace Butterfly.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListRemoveComposer : ServerPacket
    {
        public FurniListRemoveComposer(int Id)
            : base(ServerPacketHeader.USER_FURNITURE_REMOVE)
        {
            this.WriteInteger(Id);
        }
    }
}
