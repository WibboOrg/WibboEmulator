using Butterfly.HabboHotel.Catalog.Utilities;
using Butterfly.HabboHotel.Items;

namespace Butterfly.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListAddComposer : ServerPacket
    {
        public FurniListAddComposer(Item Item)
            : base(ServerPacketHeader.USER_FURNITURE_ADD)
        {
            this.WriteInteger(Item.Id);
            this.WriteString(Item.GetBaseItem().Type.ToString().ToUpper());
            this.WriteInteger(Item.Id);
            this.WriteInteger(Item.GetBaseItem().SpriteId);

            if (Item.Limited > 0)
            {
                this.WriteInteger(1);
                this.WriteInteger(256);
                this.WriteString(Item.ExtraData);
                this.WriteInteger(Item.Limited);
                this.WriteInteger(Item.LimitedStack);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }

            this.WriteBoolean(Item.GetBaseItem().AllowEcotronRecycle);
            this.WriteBoolean(Item.GetBaseItem().AllowTrade);
            this.WriteBoolean(Item.Limited == 0 ? Item.GetBaseItem().AllowInventoryStack : false);
            this.WriteBoolean(ItemUtility.IsRare(Item));
            this.WriteInteger(-1);//Seconds to expiration.
            this.WriteBoolean(true);
            this.WriteInteger(-1);//Item RoomId

            if (!Item.IsWallItem)
            {
                this.WriteString(string.Empty);
                this.WriteInteger(0);
            }
        }
    }
}
