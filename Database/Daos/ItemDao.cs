using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ItemDao
    {
        internal static int insertItem(IQueryAdapter dbClient, int itemId, int userId, string extraData)
        {
            dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (@baseId, @habboId, @extra_data)");
            dbClient.AddParameter("baseId", itemId);
            dbClient.AddParameter("habboId", userId);
            dbClient.AddParameter("extra_data", extraData);

            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static int deleteItem(IQueryAdapter dbClient, int itemId)
        {
            dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @deleteId LIMIT 1");
            dbClient.AddParameter("deleteId", itemId);
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE id = '" + ItemId + "'");
        }
       
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM items WHERE id = '" + ItemId + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE items SET room_id = '" + Room.Id + "', user_id = '" + Room.RoomData.OwnerId + "' WHERE id = '" + ItemId + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = '" + Exchange.Id + "'");
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE items SET extra_data = @extraData WHERE id = @ID LIMIT 1");
            dbClient.AddParameter("extraData", Item.ExtraData);
            dbClient.AddParameter("ID", Item.Id);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = '" + Present.Id + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE items SET base_item = @BaseItem, extra_data = @edata WHERE id = @itemId LIMIT 1");
            dbClient.AddParameter("itemId", Present.Id);
            dbClient.AddParameter("BaseItem", Row["base_id"]);
            dbClient.AddParameter("edata", Row["extra_data"]);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
            dbClient.AddParameter("itemId", Present.Id);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE items SET room_id = '" + room.Id + "', user_id = '" + room.RoomData.OwnerId + "' WHERE id = '" + Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM items WHERE items.id = '" + roomItem.Id + "'");
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '" + RoomId + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO items (user_id, room_id, base_item, extra_data, x, y, z, rot) SELECT '" + this.GetHabbo().Id + "', '" + RoomId + "', base_item, extra_data, x, y, z, rot FROM items WHERE room_id = '5328079'");
        }

        internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET base_item = @baseid WHERE id = '" + Present.Id + "'");
        dbClient.AddParameter("baseid", LotData.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
        dbClient.AddParameter("itemId", Present.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
        dbClient.AddParameter("itemId", Present.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET base_item = @baseid WHERE id = '" + Present.Id + "'");
        dbClient.AddParameter("baseid", LotData.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
        dbClient.AddParameter("itemId", Present.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
        dbClient.AddParameter("itemId", Present.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET base_item = @baseid, extra_data = @extradata WHERE id = '" + Present.Id + "'");
        dbClient.AddParameter("baseid", LotData.Id);
        dbClient.AddParameter("extradata", ExtraData);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
        dbClient.AddParameter("itemId", Present.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
        dbClient.AddParameter("itemId", Present.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET base_item = @baseid WHERE id = '" + Present.Id + "'");
        dbClient.AddParameter("baseid", LotData.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
        dbClient.AddParameter("itemId", Present.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
        dbClient.AddParameter("itemId", Present.Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO items (base_item,user_id,extra_data) VALUES (@did,@uid,@extra_data)");
        dbClient.AddParameter("did", Data.Id);
        dbClient.AddParameter("uid", Habbo.Id);
        dbClient.AddParameter("extra_data", ExtraData);
        Item.Id = Convert.ToInt32(dbClient.InsertQuery());
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO items (id,base_item,user_id,extra_data) VALUES (@id, @did,@uid,@extra_data)");
        dbClient.AddParameter("id", ItemId);
        dbClient.AddParameter("did", Data.Id);
        dbClient.AddParameter("uid", Habbo.Id);
        dbClient.AddParameter("extra_data", ExtraData);
        InsertId = Convert.ToInt32(dbClient.InsertQuery());
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO items (base_item,user_id,extra_data) VALUES (@did,@uid,@flags);");
        dbClient.AddParameter("did", Data.Id);
        dbClient.AddParameter("uid", Habbo.Id);
        dbClient.AddParameter("flags", ExtraData);
        Convert.ToInt32(dbClient.InsertQuery())
}
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO items (base_item,user_id,extra_data) VALUES(@did,@uid,@flags);");
        dbClient.AddParameter("did", Data.Id);
        dbClient.AddParameter("uid", Habbo.Id);
        dbClient.AddParameter("flags", "");
        int Item1Id = Convert.ToInt32(dbClient.InsertQuery());
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO items (base_item,user_id,extra_data) VALUES(@did,@uid,@flags);");
        dbClient.AddParameter("did", Data.Id);
        dbClient.AddParameter("uid", Habbo.Id);
        dbClient.AddParameter("flags", Item1Id.ToString());
        int Item2Id = Convert.ToInt32(dbClient.InsertQuery());
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT room_id FROM items WHERE id = '" + TeleId + "' LIMIT 1");
        DataRow row = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, base_item FROM items WHERE room_id = '" + OldRoomId + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO items (user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos)" +
                " SELECT '" + Session.GetHabbo().Id + "', '" + RoomId + "', base_item, extra_data, x, y, z, rot, wall_pos FROM items WHERE id = '" + OldItemId + "'");
        int ItemId = Convert.ToInt32(dbClient.InsertQuery());
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id FROM items WHERE id IN (SELECT item_id FROM items_limited WHERE limited_number = '" + LimitedNumber + "' AND limited_stack = '" + LimitedStack + "') AND base_item = '" + Item.ItemId + "' LIMIT 1");
        DataRow Row = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE items SET room_id = '0', user_id = '" + this._room.RoomData.OwnerId + "' WHERE room_id = '" + this._room.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE items, items_limited, user_presents, room_items_moodlight, tele_links, wired_items FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '0' AND user_id = '" + this.UserId + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE items, items_limited, user_presents, room_items_moodlight, tele_links, wired_items FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '0' AND user_id = '" + this.UserId + "' AND base_item NOT IN (SELECT id FROM furniture WHERE is_rare = '1')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT items.id, items.base_item, items.extra_data, items_limited.limited_number, items_limited.limited_stack FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.user_id = @userid AND items.room_id = '0'");
        dbClient.AddParameter("userid", this.UserId);
        table1 = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT items.id, items.base_item, items.extra_data, items_limited.limited_number, items_limited.limited_stack FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.user_id = @userid AND items.room_id = '0'");
        dbClient.AddParameter("userid", this.UserId);
        table1 = dbClient.GetTable();
    }

    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE items SET room_id = '0', user_id = '" + this.UserId + "' WHERE id = '" + Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE items SET room_id = '0', user_id = '" + this.UserId + "' WHERE id = '" + item.Id + "'");
    }
    }
}
