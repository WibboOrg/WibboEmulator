namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FurniListNotificationComposer : ServerPacket
    {
        public FurniListNotificationComposer(int Id, int Type)
            : base(ServerPacketHeader.UNSEEN_ITEMS)
        {
            this.WriteInteger(1);
            this.WriteInteger(Type);
            this.WriteInteger(1);
            this.WriteInteger(Id);
        }
    }
}
