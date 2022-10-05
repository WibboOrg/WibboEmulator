namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.Items;

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

    public FurniListNotificationComposer(List<Item> items, int Type)
        : base(ServerPacketHeader.UNSEEN_ITEMS)
    {
        this.WriteInteger(1);
        this.WriteInteger(Type);
        this.WriteInteger(items.Count);
        foreach (var item in items)
        {
            this.WriteInteger(item.Id);
        }
    }
}
