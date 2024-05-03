namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay.Troc;

using WibboEmulator.Games.Roleplays.Item;

internal sealed class RpTrocUpdateItemsComposer : ServerPacket
{
    public RpTrocUpdateItemsComposer(int userId, Dictionary<int, int> items)
      : base(ServerPacketHeader.RP_TROC_UPDATE_ITEMS)
    {
        this.WriteInteger(userId);
        this.WriteInteger(items.Count);

        foreach (var item in items)
        {
            var rpItem = RPItemManager.GetItem(item.Key);

            this.WriteInteger(item.Key);
            this.WriteString((rpItem == null) ? "" : rpItem.Name);
            this.WriteString((rpItem == null) ? "" : rpItem.Desc);
            this.WriteInteger(item.Value);
        }
    }
}
