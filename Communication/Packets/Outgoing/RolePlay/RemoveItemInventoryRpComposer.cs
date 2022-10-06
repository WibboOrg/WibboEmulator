namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay;

internal class RemoveItemInventoryRpComposer : ServerPacket
{
    public RemoveItemInventoryRpComposer(int itemId, int count)
      : base(ServerPacketHeader.REMOVE_ITEM_INVENTORY_RP)
    {
        this.WriteInteger(itemId);
        this.WriteInteger(count);
    }
}
