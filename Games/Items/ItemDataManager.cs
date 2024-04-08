namespace WibboEmulator.Games.Items;
using System.Data;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos.Item;

public class ItemDataManager
{
    private readonly Dictionary<int, ItemData> _items;
    private readonly Dictionary<int, ItemData> _gifts;

    public ItemDataManager()
    {
        this._items = new Dictionary<int, ItemData>();
        this._gifts = new Dictionary<int, ItemData>();
    }

    public void Initialize(IDbConnection dbClient)
    {
        if (this._items.Count > 0)
        {
            this._items.Clear();
        }

        if (this._gifts.Count > 0)
        {
            this._gifts.Clear();
        }

        var itemBaseList = ItemBaseDao.GetAll(dbClient);

        if (itemBaseList.Count != 0)
        {
            foreach (var itemBase in itemBaseList)
            {
                try
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

                    if (!this._gifts.ContainsKey(spriteID) && interactionType == InteractionType.GIFT)
                    {
                        this._gifts.Add(spriteID, itemData);
                    }

                    if (!this._items.ContainsKey(id))
                    {
                        this._items.Add(id, itemData);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogCriticalException(ex.ToString());
                }
            }
        }

        Console.WriteLine("Item Manager -> LOADED");
    }

    public bool GetGift(int spriteId, out ItemData item)
    {
        if (this._gifts.TryGetValue(spriteId, out item))
        {
            return true;
        }

        return false;
    }

    public bool GetItem(int id, out ItemData item)
    {
        if (this._items.TryGetValue(id, out item))
        {
            return true;
        }

        return false;
    }

    public ItemData GetItemByName(string name)
    {
        foreach (var item in this._items.Values)
        {
            if (item.ItemName == name)
            {
                return item;
            }
        }
        return null;
    }
}
