namespace WibboEmulator.Communication.Packets.Outgoing.Economy;

using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Games.Badges;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.Items;

internal sealed class EconomyCenterComposer : ServerPacket
{
    public EconomyCenterComposer(List<EmulatorEconomyCategoryEntity> economyCategoryList, List<EmulatorEconomyEntity> economyItemList)
         : base(ServerPacketHeader.ECONOMY_CENTER)
    {
        var tabCategories = economyCategoryList.FindAll(x => x.ParentId == 0);
        this.WriteInteger(tabCategories.Count);

        foreach (var tabCategory in tabCategories)
        {
            this.WriteInteger(tabCategory.Id);
            this.WriteString(tabCategory.Caption);

            var subCategories = economyCategoryList.FindAll(x => x.ParentId == tabCategory.Id);
            this.WriteInteger(subCategories.Count);

            foreach (var subCategory in subCategories)
            {
                this.WriteInteger(subCategory.Id);
                this.WriteInteger(subCategory.IconImage);
                this.WriteString(subCategory.Caption);
            }
        }

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
                        this.WriteInteger(0);
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
                        this.WriteInteger(1);
                        this.WriteInteger((int)ObjectDataKey.STRING_KEY);
                        this.WriteInteger(2);
                        this.WriteString(item.ExtraData);
                        this.WriteString(BadgeManager.AmountWinwinsBadge(item.ExtraData).ToString());
                        break;

                    case InteractionType.TROC_BANNER:
                        this.WriteInteger(2);
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
