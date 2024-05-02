namespace WibboEmulator.Games.Items;
using System.Data;
using WibboEmulator.Database.Daos.Item;

public static class ItemManager
{
    private static readonly Dictionary<int, ItemData> Items = new();
    private static readonly Dictionary<int, ItemData> Gifts = new();

    public static void Initialize(IDbConnection dbClient)
    {
        Items.Clear();
        Gifts.Clear();

        var itemBaseList = ItemBaseDao.GetAll(dbClient);

        foreach (var itemBase in itemBaseList)
        {
            var id = itemBase.Id;
            var spriteID = itemBase.SpriteId;
            var itemName = itemBase.ItemName;
            var type = itemBase.Type;
            var width = itemBase.Width;
            var length = itemBase.Length;
            var height = itemBase.StackHeight;
            var allowStack = itemBase.CanStack;
            var allowWalk = itemBase.IsWalkable;
            var allowSit = itemBase.CanSit;
            var allowRecycle = itemBase.AllowRecycle;
            var allowTrade = itemBase.AllowTrade;
            var allowGift = itemBase.AllowGift;
            var allowInventoryStack = itemBase.AllowInventoryStack;
            var allowMarketplaceSell = itemBase.AllowMarketplaceSell;
            var interactionType = InteractionTypes.GetTypeFromString(itemBase.InteractionType);
            var cycleCount = itemBase.InteractionModesCount;
            var vendingIDS = itemBase.VendingIds;
            var heightAdjustable = itemBase.HeightAdjustable;
            var effectId = itemBase.EffectId;
            var isRare = itemBase.IsRare;
            var rarityLevel = itemBase.RarityLevel;
            var amount = itemBase.Amount ?? -1;

            var itemData = new ItemData(id, spriteID, itemName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowGift, allowInventoryStack, allowMarketplaceSell, interactionType, cycleCount, vendingIDS, heightAdjustable, effectId, isRare, rarityLevel, amount);

            if (!Gifts.ContainsKey(spriteID) && interactionType == InteractionType.GIFT)
            {
                Gifts.Add(spriteID, itemData);
            }

            if (!Items.ContainsKey(id))
            {
                Items.Add(id, itemData);
            }
        }

        Console.WriteLine("Item Manager -> LOADED");
    }

    public static bool GetGift(int spriteId, out ItemData item)
    {
        if (Gifts.TryGetValue(spriteId, out item))
        {
            return true;
        }

        return false;
    }

    public static bool GetItem(int id, out ItemData item)
    {
        if (Items.TryGetValue(id, out item))
        {
            return true;
        }

        return false;
    }

    public static ItemData GetItemByName(string name)
    {
        foreach (var item in Items.Values)
        {
            if (item.ItemName == name)
            {
                return item;
            }
        }
        return null;
    }
}
