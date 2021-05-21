namespace Butterfly.Communication.Packets.Outgoing.Structure
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
