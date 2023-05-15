namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.Items;

internal sealed class UnseenItemsComposer : ServerPacket
{
    public UnseenItemsComposer(int id, UnseenItemsType type)
        : base(ServerPacketHeader.UNSEEN_ITEMS)
    {
        this.WriteInteger(1);
        this.WriteInteger((int)type);
        this.WriteInteger(1);
        this.WriteInteger(id);
    }

    public UnseenItemsComposer(List<Item> items, UnseenItemsType type)
        : base(ServerPacketHeader.UNSEEN_ITEMS)
    {
        this.WriteInteger(1);
        this.WriteInteger((int)type);
        this.WriteInteger(items.Count);
        foreach (var item in items)
        {
            this.WriteInteger(item.Id);
        }
    }
}

public enum UnseenItemsType
{
    Furni = 1,
    Rentable = 2,
    Pet = 3,
    Badge = 4,
    Bot = 5,
    Games = 6,
    Banner = 7

}