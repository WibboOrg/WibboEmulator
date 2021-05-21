using Butterfly.HabboHotel.Roleplay;
using Butterfly.HabboHotel.Roleplay.Player;
using System.Collections.Concurrent;

namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class LoadInventoryRpComposer : ServerPacket
    {
        public LoadInventoryRpComposer(ConcurrentDictionary<int, RolePlayInventoryItem> Items)
          : base(9)
        {
            this.WriteInteger(Items.Count);

            foreach (RolePlayInventoryItem Item in Items.Values)
            {
                RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Item.ItemId);

                this.WriteInteger(Item.ItemId);
                this.WriteString(RpItem.Name);
                this.WriteString(RpItem.Desc);
                this.WriteInteger(Item.Count);
                this.WriteInteger((int)RpItem.Category);
                this.WriteInteger(RpItem.UseType);
            }
        }
    }
}