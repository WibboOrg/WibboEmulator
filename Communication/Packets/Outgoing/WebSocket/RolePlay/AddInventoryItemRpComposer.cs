using Butterfly.HabboHotel.Roleplay;

namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class AddInventoryItemRpComposer : ServerPacket
    {
        public AddInventoryItemRpComposer(RPItem Item, int pCount)
          : base(10)
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
