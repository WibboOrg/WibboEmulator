using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;


namespace Butterfly.HabboHotel.Items
{
    public class ItemDataManager
    {
        public Dictionary<int, ItemData> _items;
        public Dictionary<int, ItemData> _gifts;

        public ItemDataManager()
        {
            this._items = new Dictionary<int, ItemData>();
            this._gifts = new Dictionary<int, ItemData>();
        }

        public void Init()
        {
            if (this._items.Count > 0)
            {
                this._items.Clear();
            }

            if (this._gifts.Count > 0)
            {
                this._gifts.Clear();
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id, sprite_id, item_name, type, width, length, stack_height, can_stack, is_walkable, can_sit, allow_recycle, allow_trade, allow_gift, allow_inventory_stack, interaction_type, interaction_modes_count, vending_ids, height_adjustable, effect_id, is_rare FROM `furniture`");
                DataTable ItemData = dbClient.GetTable();

                if (ItemData != null)
                {
                    foreach (DataRow Row in ItemData.Rows)
                    {
                        try
                        {
                            int id = Convert.ToInt32(Row["id"]);
                            int spriteID = Convert.ToInt32(Row["sprite_id"]);
                            string itemName = Convert.ToString(Row["item_name"]);
                            string type = Row["type"].ToString();
                            int width = Convert.ToInt32(Row["width"]);
                            int length = Convert.ToInt32(Row["length"]);
                            double height = Convert.ToDouble(Row["stack_height"]);
                            bool allowStack = ButterflyEnvironment.EnumToBool(Row["can_stack"].ToString());
                            bool allowWalk = ButterflyEnvironment.EnumToBool(Row["is_walkable"].ToString());
                            bool allowSit = ButterflyEnvironment.EnumToBool(Row["can_sit"].ToString());
                            bool allowRecycle = ButterflyEnvironment.EnumToBool(Row["allow_recycle"].ToString());
                            bool allowTrade = ButterflyEnvironment.EnumToBool(Row["allow_trade"].ToString());
                            bool allowGift = Convert.ToInt32(Row["allow_gift"]) == 1;
                            bool allowInventoryStack = ButterflyEnvironment.EnumToBool(Row["allow_inventory_stack"].ToString());
                            InteractionType interactionType = InteractionTypes.GetTypeFromString(Convert.ToString(Row["interaction_type"]));
                            int cycleCount = Convert.ToInt32(Row["interaction_modes_count"]);
                            string vendingIDS = Convert.ToString(Row["vending_ids"]);
                            string heightAdjustable = Convert.ToString(Row["height_adjustable"]);
                            int EffectId = Convert.ToInt32(Row["effect_id"]);
                            bool IsRare = ButterflyEnvironment.EnumToBool(Row["is_rare"].ToString());

                            if (!this._gifts.ContainsKey(spriteID) && interactionType == InteractionType.GIFT)
                            {
                                this._gifts.Add(spriteID, new ItemData(id, spriteID, itemName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowGift, allowInventoryStack, interactionType, cycleCount, vendingIDS, heightAdjustable, EffectId, IsRare));
                            }

                            if (!this._items.ContainsKey(id))
                            {
                                this._items.Add(id, new ItemData(id, spriteID, itemName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowGift, allowInventoryStack, interactionType, cycleCount, vendingIDS, heightAdjustable, EffectId, IsRare));
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
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
            foreach (ItemData item in this._items.Values)
            {
                if (item.ItemName == name)
                {
                    return item;
                }
            }
            return null;
        }
    }
}