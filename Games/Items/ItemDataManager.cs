namespace WibboEmulator.Games.Items;
using System.Data;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Interfaces;

public class ItemDataManager
{
    private readonly Dictionary<int, ItemData> _items;
    private readonly Dictionary<int, ItemData> _gifts;

    public ItemDataManager()
    {
        this._items = new Dictionary<int, ItemData>();
        this._gifts = new Dictionary<int, ItemData>();
    }

    public void Init(IQueryAdapter dbClient)
    {
        if (this._items.Count > 0)
        {
            this._items.Clear();
        }

        if (this._gifts.Count > 0)
        {
            this._gifts.Clear();
        }

        var itemDataTable = ItemBaseDao.GetAll(dbClient);

        if (itemDataTable != null)
        {
            foreach (DataRow row in itemDataTable.Rows)
            {
                try
                {
                    var id = Convert.ToInt32(row["id"]);
                    var spriteID = Convert.ToInt32(row["sprite_id"]);
                    var itemName = Convert.ToString(row["item_name"]);
                    var type = row["type"].ToString();
                    var width = Convert.ToInt32(row["width"]);
                    var length = Convert.ToInt32(row["length"]);
                    var height = Convert.ToDouble(row["stack_height"]);
                    var allowStack = WibboEnvironment.EnumToBool(row["can_stack"].ToString());
                    var allowWalk = WibboEnvironment.EnumToBool(row["is_walkable"].ToString());
                    var allowSit = WibboEnvironment.EnumToBool(row["can_sit"].ToString());
                    var allowRecycle = WibboEnvironment.EnumToBool(row["allow_recycle"].ToString());
                    var allowTrade = WibboEnvironment.EnumToBool(row["allow_trade"].ToString());
                    var allowGift = Convert.ToInt32(row["allow_gift"]) == 1;
                    var allowInventoryStack = WibboEnvironment.EnumToBool(row["allow_inventory_stack"].ToString());
                    var interactionType = InteractionTypes.GetTypeFromString(Convert.ToString(row["interaction_type"]));
                    var cycleCount = Convert.ToInt32(row["interaction_modes_count"]);
                    var vendingIDS = Convert.ToString(row["vending_ids"]);
                    var heightAdjustable = Convert.ToString(row["height_adjustable"]);
                    var effectId = Convert.ToInt32(row["effect_id"]);
                    var isRare = WibboEnvironment.EnumToBool(row["is_rare"].ToString());
                    var rarityLevel = Convert.ToInt32(row["rarity_level"].ToString());
                    var amount = !DBNull.Value.Equals(row["amount"]) ? Convert.ToInt32(Convert.ToString(row["amount"])) : -1;

                    var itemData = new ItemData(id, spriteID, itemName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowGift, allowInventoryStack, interactionType, cycleCount, vendingIDS, heightAdjustable, effectId, isRare, rarityLevel, amount);

                    if (!this._gifts.ContainsKey(spriteID) && interactionType == InteractionType.GIFT)
                    {
                        this._gifts.Add(spriteID, itemData);
                    }

                    if (!this._items.ContainsKey(id))
                    {
                        this._items.Add(id, itemData);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
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
