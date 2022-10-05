namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay;
using System.Collections.Concurrent;
using WibboEmulator.Games.Roleplay.Player;

internal class LoadInventoryRpComposer : ServerPacket
{
    public LoadInventoryRpComposer(ConcurrentDictionary<int, RolePlayInventoryItem> Items)
      : base(ServerPacketHeader.LOAD_INVENTORY_RP)
    {
        this.WriteInteger(Items.Count);

        foreach (var Item in Items.Values)
        {
            var RpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Item.ItemId);

            this.WriteInteger(Item.ItemId);
            this.WriteString(RpItem.Name);
            this.WriteString(RpItem.Desc);
            this.WriteInteger(Item.Count);
            this.WriteInteger((int)RpItem.Category);
            this.WriteInteger(RpItem.UseType);
        }
    }
}