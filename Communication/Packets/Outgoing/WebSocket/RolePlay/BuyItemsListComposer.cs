using Butterfly.Game.Roleplay;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class BuyItemsListComposer : ServerPacket
    {
        public BuyItemsListComposer(List<RPItem> ItemsBuy)
          : base(8)
        {
            this.WriteInteger(ItemsBuy.Count);

            foreach (RPItem Item in ItemsBuy)
            {
                this.WriteInteger(Item.Id);
                this.WriteString(Item.Name);
                this.WriteString(Item.Desc);
                this.WriteInteger(Item.Price);
            }
        }
    }
}