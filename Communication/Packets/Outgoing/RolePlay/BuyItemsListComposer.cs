namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay;
using WibboEmulator.Games.Roleplays.Item;

internal class BuyItemsListComposer : ServerPacket
{
    public BuyItemsListComposer(List<RPItem> itemsBuy)
      : base(ServerPacketHeader.BUY_ITEMS_LIST)
    {
        this.WriteInteger(itemsBuy.Count);

        foreach (var item in itemsBuy)
        {
            this.WriteInteger(item.Id);
            this.WriteString(item.Name);
            this.WriteString(item.Desc);
            this.WriteInteger(item.Price);
        }
    }
}
