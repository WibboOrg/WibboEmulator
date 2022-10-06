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

        var ItemData = ItemBaseDao.GetAll(dbClient);

        if (ItemData != null)
        {
            foreach (DataRow Row in ItemData.Rows)
            {
                try
                {
                    var id = Convert.ToInt32(Row["id"]);
                    var spriteID = Convert.ToInt32(Row["sprite_id"]);
                    var itemName = Convert.ToString(Row["item_name"]);
                    var type = Row["type"].ToString();
                    var width = Convert.ToInt32(Row["width"]);
                    var length = Convert.ToInt32(Row["length"]);
                    var height = Convert.ToDouble(Row["stack_height"]);
                    var allowStack = WibboEnvironment.EnumToBool(Row["can_stack"].ToString());
                    var allowWalk = WibboEnvironment.EnumToBool(Row["is_walkable"].ToString());
                    var allowSit = WibboEnvironment.EnumToBool(Row["can_sit"].ToString());
                    var allowRecycle = WibboEnvironment.EnumToBool(Row["allow_recycle"].ToString());
                    var allowTrade = WibboEnvironment.EnumToBool(Row["allow_trade"].ToString());
                    var allowGift = Convert.ToInt32(Row["allow_gift"]) == 1;
                    var allowInventoryStack = WibboEnvironment.EnumToBool(Row["allow_inventory_stack"].ToString());
                    var interactionType = InteractionTypes.GetTypeFromString(Convert.ToString(Row["interaction_type"]));
                    var cycleCount = Convert.ToInt32(Row["interaction_modes_count"]);
                    var vendingIDS = Convert.ToString(Row["vending_ids"]);
                    var heightAdjustable = Convert.ToString(Row["height_adjustable"]);
                    var effectId = Convert.ToInt32(Row["effect_id"]);
                    var isRare = WibboEnvironment.EnumToBool(Row["is_rare"].ToString());
                    var rarityLevel = Convert.ToInt32(Row["rarity_level"].ToString());
                    var amount = !DBNull.Value.Equals(Row["amount"]) ? Convert.ToInt32(Convert.ToString(Row["amount"])) : -1;

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

    public bool GetGift(int SpriteId, out ItemData Item)
    {
        if (this._gifts.TryGetValue(SpriteId, out Item))
        {
            return true;
        }

        return false;
    }

    public bool GetItem(int Id, out ItemData Item)
    {
        if (this._items.TryGetValue(Id, out Item))
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