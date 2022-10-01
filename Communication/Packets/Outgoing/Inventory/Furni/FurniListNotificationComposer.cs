using WibboEmulator.Games.Items;

namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni
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

        public FurniListNotificationComposer(List<Item> items, int Type)
            : base(ServerPacketHeader.UNSEEN_ITEMS)
        {
            this.WriteInteger(1);
            this.WriteInteger(Type);
            this.WriteInteger(items.Count);
            foreach(Item item in items)
                this.WriteInteger(item.Id);
        }
    }
}
