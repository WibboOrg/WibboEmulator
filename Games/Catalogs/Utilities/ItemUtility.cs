namespace WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.Items;

public static class ItemUtility
{
    public static bool CanGiftItem(CatalogItem item)
    {
        if (item.Data.InteractionType == InteractionType.TROPHY)
        {
            return true;
        }

        if (!item.Data.AllowGift || item.IsLimited || item.Amount > 1 || item.Data.InteractionType == InteractionType.EXCHANGE ||
            item.Data.InteractionType == InteractionType.BADGE || (item.Data.Type != 's' && item.Data.Type != 'i') || item.CostWibboPoints > 0 ||
            item.Data.InteractionType == InteractionType.TELEPORT || item.Data.InteractionType == InteractionType.TELEPORT_ARROW)
        {
            return false;
        }

        if (item.Data.IsRare || item.Data.RarityLevel > 0)
        {
            return false;
        }

        if (item.Data.InteractionType == InteractionType.PET)
        {
            return false;
        }

        return true;
    }

    public static bool CanSelectAmount(CatalogItem item)
    {
        if (item.IsLimited || item.Amount > 1 || item.Data.InteractionType == InteractionType.EXCHANGE || !item.HaveOffer || item.Data.InteractionType == InteractionType.BADGE)
        {
            return false;
        }

        return true;
    }

    public static int GetSaddleId(int saddle) => saddle switch
    {
        10 => 7544143,
        _ => 2804,
    };

    public static bool IsRare(Item item)
    {
        if (item.Data.InteractionType == InteractionType.EXCHANGE)
        {
            return false;
        }

        if (item.Limited > 0)
        {
            return true;
        }

        if (item.Data.IsRare || item.Data.RarityLevel > 0)
        {
            return true;
        }

        return false;
    }
}
