namespace WibboEmulator.Communication.Packets.Outgoing.Economy;

using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Games.Badges;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.Items;

internal sealed class EconomyCenterComposer : ServerPacket
{
    public EconomyCenterComposer(List<EmulatorEconomyEntity> economyItemList)
         : base(ServerPacketHeader.ECONOMY_CENTER)
    {
        this.WriteInteger(4);
        this.WriteString("Édition limitée");
        this.WriteString("Mobilier");
        this.WriteString("Badge");
        this.WriteString("Bannière");

        this.WriteInteger(economyItemList.Count);
        foreach (var item in economyItemList)
        {
            if (ItemManager.GetItem(item.ItemId, out var itemData))
            {
                this.WriteInteger(item.Id);
                this.WriteInteger(item.CategoryId);
                this.WriteInteger(item.AveragePrice);
                this.WriteInteger(item.ItemId);

                switch (itemData.InteractionType)
                {
                    default:
                        this.WriteInteger((int)ObjectDataKey.MAP_KEY);

                        var totalSets = 1;
                        if (itemData.RarityLevel > RaretyLevelType.None)
                        {
                            totalSets++;
                        }

                        if (itemData.Amount >= 0)
                        {
                            totalSets++;
                        }

                        this.WriteInteger(totalSets);

                        if (itemData.RarityLevel > RaretyLevelType.None)
                        {
                            this.WriteString("rarity");
                            this.WriteString(((int)itemData.RarityLevel).ToString());
                        }

                        if (itemData.Amount >= 0)
                        {
                            this.WriteString("amount");
                            this.WriteString(itemData.Amount.ToString());
                        }

                        this.WriteString("state");
                        this.WriteString(item.ExtraData);
                        break;
                    case InteractionType.BADGE_TROC:
                        this.WriteInteger((int)ObjectDataKey.STRING_KEY);
                        this.WriteInteger(2);
                        this.WriteString(item.ExtraData);
                        this.WriteString(BadgeManager.AmountWinwinsBadge(item.ExtraData).ToString());
                        break;

                    case InteractionType.TROC_BANNER:
                        if (!int.TryParse(item.ExtraData, out var bannerId) ||
                        !BannerManager.TryGetBannerById(bannerId, out var banner))
                        {
                            this.WriteInteger((int)ObjectDataKey.LEGACY_KEY);
                            this.WriteString(item.ExtraData);
                            break;
                        }
                        this.WriteInteger((int)ObjectDataKey.NUMBER_KEY);
                        this.WriteInteger(2);
                        this.WriteInteger(banner.Id);
                        this.WriteInteger(banner.HaveLayer ? 1 : -1);
                        break;
                }
            }
            else
            {
                this.WriteInteger(-1);
            }
        }
    }
}
