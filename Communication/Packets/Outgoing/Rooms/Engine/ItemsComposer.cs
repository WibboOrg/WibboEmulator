using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemsComposer : ServerPacket
    {
        public ItemsComposer(Item[] Objects, Room Room)
            : base(ServerPacketHeader.ITEM_WALL)
        {
            this.WriteInteger(1);
            this.WriteInteger(Room.RoomData.OwnerId);
            this.WriteString(Room.RoomData.OwnerName);

            this.WriteInteger(Objects.Length);

            foreach (Item Item in Objects)
            {
                this.WriteWallItem(Item, Room.RoomData.OwnerId);
            }
        }

        private void WriteWallItem(Item Item, int UserId)
        {
            this.WriteString(Item.Id.ToString());
            this.WriteInteger(Item.GetBaseItem().SpriteId);

            try
            {
                this.WriteString(Item.WallCoord);
            }
            catch
            {
                this.WriteString("");
            }

            ItemBehaviourUtility.GenerateWallExtradata(Item, this);

            this.WriteInteger(-1);
            this.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            this.WriteInteger(UserId);
        }
    }
}
