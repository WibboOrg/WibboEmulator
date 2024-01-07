namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Rooms;

internal sealed class DuplicateRoom : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var oldRoomId = room.Id;
        int roomId;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            room.RoomItemHandling.SaveFurniture(dbClient);

            roomId = RoomDao.InsertDuplicate(dbClient, oldRoomId, session.User.Username);

            RoomModelCustomDao.InsertDuplicate(dbClient, roomId, oldRoomId);

            var furniIdAllow = new List<int>();

            var catalogItemTable = CatalogItemDao.GetItemIdByRank(dbClient, "");
            foreach (DataRow dataRow in catalogItemTable.Rows)
            {
                _ = int.TryParse(dataRow["item_id"].ToString(), out var itemId);
                if (!furniIdAllow.Contains(itemId))
                {
                    furniIdAllow.Add(itemId);
                }
            }

            var newItemsId = new Dictionary<int, int>();
            var wiredId = new List<int>();
            var teleportId = new List<int>();

            var itemTable = ItemDao.GetAllIdAndBaseItem(dbClient, oldRoomId);
            foreach (DataRow dataRow in itemTable.Rows)
            {
                var oldItemId = Convert.ToInt32(dataRow["id"]);
                var baseID = Convert.ToInt32(dataRow["base_item"]);

                if (!furniIdAllow.Contains(baseID))
                {
                    continue;
                }

                _ = WibboEnvironment.GetGame().GetItemManager().GetItem(baseID, out var data);
                if (data == null || data.IsRare || data.RarityLevel > RaretyLevelType.None)
                {
                    continue;
                }

                var itemId = ItemDao.InsertDuplicate(dbClient, session.User.Id, roomId, oldItemId);

                newItemsId.Add(oldItemId, itemId);

                if (data.InteractionType is InteractionType.TELEPORT or InteractionType.TELEPORT_ARROW)
                {
                    teleportId.Add(oldItemId);
                }

                if (data.InteractionType == InteractionType.MOODLIGHT)
                {
                    ItemMoodlightDao.InsertDuplicate(dbClient, itemId, oldItemId);
                }

                if (WiredUtillity.TypeIsWired(data.InteractionType))
                {
                    ItemWiredDao.InsertDuplicate(dbClient, itemId, oldItemId);

                    wiredId.Add(itemId);
                }
            }

            foreach (var oldId in teleportId)
            {
                if (!newItemsId.TryGetValue(oldId, out var newId))
                {
                    continue;
                }

                var rowTele = ItemTeleportDao.GetOne(dbClient, oldId);
                if (rowTele == null)
                {
                    continue;
                }

                if (!newItemsId.TryGetValue(Convert.ToInt32(rowTele["tele_two_id"]), out var newIdTwo))
                {
                    continue;
                }

                ItemTeleportDao.Insert(dbClient, newId, newIdTwo);
            }

            foreach (var id in wiredId)
            {
                var wiredRow = ItemWiredDao.GetOne(dbClient, id);

                if (wiredRow == null)
                {
                    continue;
                }

                var triggerItems = "";

                var oldItems = (string)wiredRow["triggers_item"];

                if (oldItems.Length <= 0)
                {
                    continue;
                }

                if (oldItems.Contains(':'))
                {
                    foreach (var oldItem in oldItems.Split(';'))
                    {
                        var itemData = oldItem.Split(':');

                        if (itemData.Length != 6)
                        {
                            continue;
                        }

                        var oldId = Convert.ToInt32(itemData[0]);

                        if (!newItemsId.TryGetValue(Convert.ToInt32(oldId), out var newId))
                        {
                            continue;
                        }

                        triggerItems += newId + ":" + Convert.ToInt32(itemData[1]) + ":" + Convert.ToInt32(itemData[2]) + ":" + Convert.ToDouble(itemData[3]) + ":" + Convert.ToInt32(itemData[4]) + ":" + itemData[5] + ";";
                    }
                }
                else
                {
                    foreach (var itemData in oldItems.Split(';'))
                    {
                        if (!int.TryParse(itemData, out var oldId))
                        {
                            continue;
                        }

                        if (!newItemsId.TryGetValue(oldId, out var newId))
                        {
                            continue;
                        }

                        triggerItems += newId + ";";
                    }
                }

                if (triggerItems.Length > 0)
                {
                    triggerItems = triggerItems.Remove(triggerItems.Length - 1);
                }

                ItemWiredDao.UpdateTriggerItem(dbClient, triggerItems, id);
            }

            BotUserDao.DupliqueAllBotInRoomId(dbClient, session.User.Id, roomId, oldRoomId);

            BotPetDao.InsertDuplicate(dbClient, session.User.Id, roomId, oldRoomId);
        }

        if (!session.User.UsersRooms.Contains(roomId))
        {
            session.User.UsersRooms.Add(roomId);
        }

        session.SendPacket(new FlatCreatedComposer(roomId, "Appart " + oldRoomId + " copie"));
    }
}
