namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class CarryObjectComposer : ServerPacket
    {
        public CarryObjectComposer(int virtualID, int itemID)
            : base(ServerPacketHeader.UNIT_HAND_ITEM)
        {
            this.WriteInteger(virtualID);
            this.WriteInteger(itemID);
        }
    }
}
