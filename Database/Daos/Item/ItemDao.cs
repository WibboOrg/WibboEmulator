using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Items;
using WibboEmulator.Utilities;
using System.Collections.Concurrent;
using System.Data;

namespace WibboEmulator.Database.Daos
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
                    standardQueries.AddQuery("UPDATE `item` SET extra_data = @data" + roomItem.Id + " WHERE id = '" + roomItem.Id + "'");
                    standardQueries.AddParameter("data" + roomItem.Id, roomItem.ExtraData);
                }

                if (roomItem.IsWallItem)
                {
                    standardQueries.AddQuery("UPDATE `item` SET wall_pos = @wallpost" + roomItem.Id + " WHERE id = " + roomItem.Id);
                    standardQueries.AddParameter("wallpost" + roomItem.Id, roomItem.WallCoord);
                }
                else
                {
                    standardQueries.AddQuery("UPDATE `item` SET x=" + roomItem.X + ", y=" + roomItem.Y + ", z=" + roomItem.Z + ", rot=" + roomItem.Rotation + " WHERE id=" + roomItem.Id + "");
                }
            }

            standardQueries.Execute(dbClient);
            standardQueries.Dispose();
        }

        internal static int Insert(IQueryAdapter dbClient, int baseItem, int userId, string extraData)
        {
            dbClient.SetQuery("INSERT INTO `item` (base_item,user_id,extra_data) VALUES (@baseId, @userId, @extra_data)");
            dbClient.AddParameter("baseId", baseItem);
            dbClient.AddParameter("userId", userId);
            dbClient.AddParameter("extra_data", extraData);

            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static int Insert(IQueryAdapter dbClient, int itemId, int baseItem, int userId, string extraData)
        {
            dbClient.SetQuery("INSERT INTO `item` (id,base_item,user_id,extra_data) VALUES (@id, @did,@uid,@extra_data)");
            dbClient.AddParameter("id", itemId);
            dbClient.AddParameter("did", baseItem);
            dbClient.AddParameter("uid", userId);
            dbClient.AddParameter("extra_data", extraData);

            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static int InsertDuplicate(IQueryAdapter dbClient, int userId, int roomId, int itemId)
        {
            dbClient.SetQuery("INSERT INTO `item` (user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos)" +
                    " SELECT '" + userId + "', '" + roomId + "', base_item, extra_data, x, y, z, rot, wall_pos FROM `item` WHERE id = '" + itemId + "'");
            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void InsertDuplicate(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("INSERT INTO `item` (user_id, room_id, base_item, extra_data, x, y, z, rot) SELECT '" + userId + "', '" + roomId + "', base_item, extra_data, x, y, z, rot FROM `item` WHERE room_id = '5328079'");
        }

        internal static void Delete(IQueryAdapter dbClient, int itemId)
        {
            dbClient.RunQuery("DELETE `item`, `item_limited` FROM `item` LEFT JOIN `item_limited` ON(`item_limited`.item_id = `item`.id) WHERE id = '" + itemId + "'");
        }

        internal static void DeleteAllByRoomId(IQueryAdapter dbClient, int roomId)
        {
            dbClient.RunQuery("DELETE `item`, `item_limited` FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) LEFT JOIN `item_present` ON (`item_present`.item_id = `item`.id) LEFT JOIN `item_moodlight` ON (`item_moodlight`.item_id = `item`.id) LEFT JOIN `item_teleport` ON (tele_one_id = `item`.id) LEFT JOIN `item_wired` ON (trigger_id = `item`.id) WHERE room_id = '" + roomId + "'");
        }

        internal static void DeleteAll(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("DELETE `item`, `item_limited`, `item_present`, `item_moodlight`, `item_teleport`, `item_wired` FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) LEFT JOIN `item_present` ON (`item_present`.item_id = `item`.id) LEFT JOIN `item_moodlight` ON (`item_moodlight`.item_id = `item`.id) LEFT JOIN `item_teleport` ON (tele_one_id = `item`.id) LEFT JOIN `item_wired` ON (trigger_id = `item`.id) WHERE room_id = '0' AND user_id = '" + userId + "'");
        }

        internal static void DeleteAllWithoutRare(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("DELETE `item`, `item_limited`, `item_present`, `item_moodlight`, `item_teleport`, `item_wired` FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) LEFT JOIN `item_present` ON (`item_present`.item_id = `item`.id) LEFT JOIN `item_moodlight` ON (`item_moodlight`.item_id = `item`.id) LEFT JOIN `item_teleport` ON (tele_one_id = `item`.id) LEFT JOIN `item_wired` ON (trigger_id = `item`.id) WHERE room_id = '0' AND user_id = '" + userId + "' AND base_item NOT IN (SELECT id FROM `item_base` WHERE is_rare = '1')");
        }

        internal static void UpdateExtradata(IQueryAdapter dbClient, int itemId, string extraData)
        {
            dbClient.SetQuery("UPDATE `item` SET extra_data = @extraData WHERE id = @ID LIMIT 1");
            dbClient.AddParameter("extraData", extraData);
            dbClient.AddParameter("ID", itemId);
            dbClient.RunQuery();
        }

        internal static void UpdateBaseItem(IQueryAdapter dbClient, int itemId, int baseItem)
        {
            dbClient.SetQuery("UPDATE `item` SET base_item = @baseid WHERE id = '" + itemId + "'");
            dbClient.AddParameter("baseid", baseItem);
            dbClient.RunQuery();
        }

        internal static void UpdateBaseItemAndExtraData(IQueryAdapter dbClient, int itemId, int baseItem, string extraData)
        {
            dbClient.SetQuery("UPDATE `item` SET base_item = @baseid, extra_data = @extradata WHERE id = '" + itemId + "'");
            dbClient.AddParameter("baseid", baseItem);
            dbClient.AddParameter("extradata", extraData);
            dbClient.RunQuery();
        }

        internal static void UpdateRoomIdAndUserId(IQueryAdapter dbClient, int itemId, int roomId, int userId)
        {
            dbClient.RunQuery("UPDATE `item` SET room_id = '" + roomId + "', user_id = '" + userId + "' WHERE id = '" + itemId + "'");
        }

        internal static void UpdateRoomIdAndUserId(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("UPDATE `item` SET room_id = '0', user_id = '" + userId + "' WHERE room_id = '" + roomId + "'");
        }

        internal static void UpdateResetRoomId(IQueryAdapter dbClient, int itemId)
        {
            dbClient.SetQuery("UPDATE `item` SET room_id = '0' WHERE id = @itemId LIMIT 1");
            dbClient.AddParameter("itemId", itemId);
            dbClient.RunQuery();
        }

        internal static DataRow GetOneRoomId(IQueryAdapter dbClient, int itemId)
        {
            dbClient.SetQuery("SELECT room_id FROM `item` WHERE id = '" + itemId + "' LIMIT 1");
            return dbClient.GetRow();
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int roomId)
        {
            dbClient.SetQuery("SELECT `item`.id, `item`.user_id, `item`.room_id, `item`.base_item, `item`.extra_data, `item`.x, `item`.y, `item`.z, `item`.rot, `item`.wall_pos, `item_limited`.limited_number, `item_limited`.limited_stack FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) WHERE `item`.room_id = @roomid");
            dbClient.AddParameter("roomid", roomId);

            return dbClient.GetTable();
        }

        internal static DataTable GetAllIdAndBaseItem(IQueryAdapter dbClient, int roomId)
        {
            dbClient.SetQuery("SELECT id, base_item FROM `item` WHERE room_id = '" + roomId + "'");
            return dbClient.GetTable();
        }

        internal static DataRow GetOneLimitedId(IQueryAdapter dbClient, int limitedNumber, int itemId)
        {
            dbClient.SetQuery("SELECT id FROM `item` WHERE id IN (SELECT item_id FROM `item_limited` WHERE limited_number = '" + limitedNumber + "') AND base_item = '" + itemId + "' LIMIT 1");
            return dbClient.GetRow();
        }

        internal static DataTable GetAllByUserId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT `item`.id, `item`.base_item, `item`.extra_data, `item_limited`.limited_number, `item_limited`.limited_stack FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) WHERE `item`.user_id = @userid AND `item`.room_id = '0'");
            dbClient.AddParameter("userid", userId);
            return dbClient.GetTable();
        }
    }
}
