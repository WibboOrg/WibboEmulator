namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class RemoveItemInventoryRpComposer : ServerPacket
    {
        public RemoveItemInventoryRpComposer(int ItemId, int Count)
          : base(ServerPacketHeader.REMOVE_ITEM_INVENTORY_RP)
        {
            this.WriteInteger(ItemId);
            this.WriteInteger(Count);
        }
    }
}
