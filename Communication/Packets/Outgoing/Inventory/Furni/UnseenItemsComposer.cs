namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.Items;

internal sealed class UnseenItemsComposer : ServerPacket
{
    public UnseenItemsComposer(int id, int type)
        : base(ServerPacketHeader.UNSEEN_ITEMS)
    {
        this.WriteInteger(1);
        this.WriteInteger(type);
        this.WriteInteger(1);
        this.WriteInteger(id);
    }

    public UnseenItemsComposer(List<Item> items, int type)
        : base(ServerPacketHeader.UNSEEN_ITEMS)
    {
        this.WriteInteger(1);
        this.WriteInteger(type);
        this.WriteInteger(items.Count);
        foreach (var item in items)
        {
            this.WriteInteger(item.Id);
        }
    }
}
