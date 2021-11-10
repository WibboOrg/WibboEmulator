using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Utilities;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace Butterfly.Database.Daos
{
    class ItemDao
    {
        internal static void SaveUpdateItems(IQueryAdapter dbClient, ConcurrentDictionary<int, Item> updateItems)
        {
            QueryChunk standardQueries = new QueryChunk();

            foreach (Item roomItem in updateItems.Values)
            {
                if (!string.IsNullOrEmpty(roomItem.ExtraData))
                {
                    standardQueries.AddQuery("UPDATE items SET extra_data = @data" + roomItem.Id + " WHERE id = '" + roomItem.Id + "'");
                    standardQueries.AddParameter("data" + roomItem.Id, roomItem.ExtraData);
                }

                if (roomItem.IsWallItem)
                {
                    standardQueries.AddQuery("UPDATE items SET wall_pos = @wallpost" + roomItem.Id + " WHERE id = " + roomItem.Id);
                    standardQueries.AddParameter("wallpost" + roomItem.Id, roomItem.WallCoord);
                }
                else
                {
                    standardQueries.AddQuery("UPDATE items SET x=" + roomItem.GetX + ", y=" + roomItem.GetY + ", z=" + roomItem.GetZ + ", rot=" + roomItem.Rotation + " WHERE id=" + roomItem.Id + "");
                }
            }

            standardQueries.Execute(dbClient);
            standardQueries.Dispose();
        }

        internal static int Insert(IQueryAdapter dbClient, int baseItem, int userId, string extraData)
        {
            dbClient.SetQuery("INSERT INTO items (base_item,user_id,extra_data) VALUES (@baseId, @habboId, @extra_data)");
            dbClient.AddParameter("baseId", baseItem);
            dbClient.AddParameter("habboId", userId);
            dbClient.AddParameter("extra_data", extraData);

            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static int Insert(IQueryAdapter dbClient, int itemId, int baseItem, int userId, string extraData)
        {
            dbClient.SetQuery("INSERT INTO items (id,base_item,user_id,extra_data) VALUES (@id, @did,@uid,@extra_data)");
            dbClient.AddParameter("id", itemId);
            dbClient.AddParameter("did", baseItem);
            dbClient.AddParameter("uid", userId);
            dbClient.AddParameter("extra_data", extraData);

            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static int InsertDuplicate(IQueryAdapter dbClient, int userId, int roomId, int itemId)
        {
            dbClient.SetQuery("INSERT INTO items (user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos)" +
                    " SELECT '" + userId + "', '" + roomId + "', base_item, extra_data, x, y, z, rot, wall_pos FROM items WHERE id = '" + itemId + "'");
            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void InsertDuplicate(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("INSERT INTO items (user_id, room_id, base_item, extra_data, x, y, z, rot) SELECT '" + userId + "', '" + roomId + "', base_item, extra_data, x, y, z, rot FROM items WHERE room_id = '5328079'");
        }

        internal static void Delete(IQueryAdapter dbClient, int itemId)
        {
            dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON(items_limited.item_id = items.id) WHERE id = '" + itemId + "'");
        }

        internal static void DeleteAllByRoomId(IQueryAdapter dbClient, int roomId)
        {
            dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '" + roomId + "'");
        }

        internal static void DeleteAll(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("DELETE items, items_limited, user_presents, room_items_moodlight, tele_links, wired_items FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '0' AND user_id = '" + userId + "'");
        }

        internal static void DeleteAllWithoutRare(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("DELETE items, items_limited, user_presents, room_items_moodlight, tele_links, wired_items FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '0' AND user_id = '" + userId + "' AND base_item NOT IN (SELECT id FROM furniture WHERE is_rare = '1')");
        }

        internal static void UpdateExtradata(IQueryAdapter dbClient, int itemId, string extraData)
        {
            dbClient.SetQuery("UPDATE items SET extra_data = @extraData WHERE id = @ID LIMIT 1");
            dbClient.AddParameter("extraData", extraData);
            dbClient.AddParameter("ID", itemId);
            dbClient.RunQuery();
        }

        internal static void UpdateBaseItem(IQueryAdapter dbClient, int itemId, int baseItem)
        {
            dbClient.SetQuery("UPDATE items SET base_item = @baseid WHERE id = '" + itemId + "'");
            dbClient.AddParameter("baseid", baseItem);
            dbClient.RunQuery();
        }

        internal static void UpdateBaseItemAndExtraData(IQueryAdapter dbClient, int itemId, int baseItem, string extraData)
        {
            dbClient.SetQuery("UPDATE items SET base_item = @baseid, extra_data = @extradata WHERE id = '" + itemId + "'");
            dbClient.AddParameter("baseid", baseItem);
            dbClient.AddParameter("extradata", extraData);
            dbClient.RunQuery();
        }

        internal static void UpdateRoomIdAndUserId(IQueryAdapter dbClient, int itemId, int roomId, int userId)
        {
            dbClient.RunQuery("UPDATE items SET room_id = '" + roomId + "', user_id = '" + userId + "' WHERE id = '" + itemId + "'");
        }

        internal static void UpdateRoomIdAndUserId(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("UPDATE items SET room_id = '0', user_id = '" + userId + "' WHERE room_id = '" + roomId + "'");
        }

        internal static void UpdateResetRoomId(IQueryAdapter dbClient, int itemId)
        {
            dbClient.SetQuery("UPDATE items SET room_id = '0' WHERE id = @itemId LIMIT 1");
            dbClient.AddParameter("itemId", itemId);
            dbClient.RunQuery();
        }

        internal static DataRow GetOneRoomId(IQueryAdapter dbClient, int itemId)
        {
            dbClient.SetQuery("SELECT room_id FROM items WHERE id = '" + itemId + "' LIMIT 1");
            return dbClient.GetRow();
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int roomId)
        {
            dbClient.SetQuery("SELECT items.id, items.user_id, items.room_id, items.base_item, items.extra_data, items.x, items.y, items.z, items.rot, items.wall_pos, items_limited.limited_number, items_limited.limited_stack FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.room_id = @roomid");
            dbClient.AddParameter("roomid", roomId);

            return dbClient.GetTable();
        }

        internal static DataTable GetAllIdAndBaseItem(IQueryAdapter dbClient, int roomId)
        {
            dbClient.SetQuery("SELECT id, base_item FROM items WHERE room_id = '" + roomId + "'");
            return dbClient.GetTable();
        }

        internal static DataRow GetOneLimitedId(IQueryAdapter dbClient, int limitedNumber, int limitedStack, int itemId)
        {
            dbClient.SetQuery("SELECT id FROM items WHERE id IN (SELECT item_id FROM items_limited WHERE limited_number = '" + limitedNumber + "' AND limited_stack = '" + limitedStack + "') AND base_item = '" + itemId + "' LIMIT 1");
            return dbClient.GetRow();
        }

        internal static DataTable GetAllByUserId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT items.id, items.base_item, items.extra_data, items_limited.limited_number, items_limited.limited_stack FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.user_id = @userid AND items.room_id = '0'");
            dbClient.AddParameter("userid", userId);
            return dbClient.GetTable();
        }
    }
}
