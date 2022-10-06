namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay;
using System.Collections.Concurrent;
using WibboEmulator.Games.Roleplay.Player;

internal class LoadInventoryRpComposer : ServerPacket
{
    public LoadInventoryRpComposer(ConcurrentDictionary<int, RolePlayInventoryItem> items)
      : base(ServerPacketHeader.LOAD_INVENTORY_RP)
    {
        this.WriteInteger(items.Count);

        foreach (var item in items.Values)
        {
            var rpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(item.ItemId);

            this.WriteInteger(item.ItemId);
            this.WriteString(rpItem.Name);
            this.WriteString(rpItem.Desc);
            this.WriteInteger(item.Count);
            this.WriteInteger((int)rpItem.Category);
            this.WriteInteger(rpItem.UseType);
        }
    }
}
