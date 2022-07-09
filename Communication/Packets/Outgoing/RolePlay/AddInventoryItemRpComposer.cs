using Wibbo.Game.Roleplay;

namespace Wibbo.Communication.Packets.Outgoing.RolePlay
{
    internal class AddInventoryItemRpComposer : ServerPacket
    {
        public AddInventoryItemRpComposer(RPItem Item, int pCount)
          : base(ServerPacketHeader.ADD_INVENTORY_ITEM_RP)
        {
            this.WriteInteger(Item.Id);
            this.WriteString(Item.Name);
            this.WriteString(Item.Desc);
            this.WriteInteger((int)Item.Category);
            this.WriteInteger(pCount);
            this.WriteInteger(Item.UseType);
        }
    }
}
