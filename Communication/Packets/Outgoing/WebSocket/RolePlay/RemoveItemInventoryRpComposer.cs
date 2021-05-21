namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class RemoveItemInventoryRpComposer : ServerPacket
    {
        public RemoveItemInventoryRpComposer(int ItemId, int Count)
          : base(11)
        {
            this.WriteInteger(ItemId);
            this.WriteInteger(Count);
        }
    }
}
