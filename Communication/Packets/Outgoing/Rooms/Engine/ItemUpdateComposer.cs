using WibboEmulator.Game.Items;

namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemUpdateComposer : ServerPacket
    {
        public ItemUpdateComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ITEM_WALL_UPDATE)
        {
            this.WriteWallItem(Item, UserId);
        }

        private void WriteWallItem(Item Item, int UserId)
        {
            this.WriteString(Item.Id.ToString());
            this.WriteInteger(Item.GetBaseItem().SpriteId);
            this.WriteString(Item.WallCoord);
            switch (Item.GetBaseItem().InteractionType)
            {
                case InteractionType.POSTIT:
                    this.WriteString(Item.ExtraData.Split(' ')[0]);
                    break;

                default:
                    this.WriteString(Item.ExtraData);
                    break;
            }
            this.WriteInteger(-1);
            this.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            this.WriteInteger(UserId);
        }
    }
}
