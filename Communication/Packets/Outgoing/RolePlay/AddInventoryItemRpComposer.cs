namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay;
using WibboEmulator.Games.Roleplays.Item;

internal class AddInventoryItemRpComposer : ServerPacket
{
    public AddInventoryItemRpComposer(RPItem item, int count)
      : base(ServerPacketHeader.ADD_INVENTORY_ITEM_RP)
    {
        this.WriteInteger(item.Id);
        this.WriteString(item.Name);
        this.WriteString(item.Desc);
        this.WriteInteger((int)item.Category);
        this.WriteInteger(count);
        this.WriteInteger(item.UseType);
    }
}
