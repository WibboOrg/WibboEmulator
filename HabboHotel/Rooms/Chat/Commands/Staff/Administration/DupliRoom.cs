using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class DupliRoom : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int OldRoomId = Room.Id;
            int RoomId;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Room.GetRoomItemHandler().SaveFurniture(dbClient);

                RoomId = RoomDao.InsertDuplicate(dbClient, OldRoomId, Session.GetHabbo().Username);

                RoomModelCustomDao.InsertDuplicate(dbClient, RoomId, OldRoomId);

                List<int> furniIdAllow = new List<int>();

                DataTable catalogItemTable = CatalogItemDao.GetItemIdByRank(dbClient, Session.GetHabbo().Rank);
                foreach (DataRow dataRow in catalogItemTable.Rows)
                {
                    int.TryParse(dataRow["item_id"].ToString(), out int itemId);
                    if (!furniIdAllow.Contains(itemId))
                    {
                        furniIdAllow.Add(itemId);
                    }
                }

                Dictionary<int, int> newItemsId = new Dictionary<int, int>();
                List<int> wiredId = new List<int>();
                List<int> teleportId = new List<int>();

                DataTable itemTable = ItemDao.GetAll(dbClient, OldRoomId);
                foreach (DataRow dataRow in itemTable.Rows)
                {
                    int OldItemId = Convert.ToInt32(dataRow[0]);
                    int baseID = Convert.ToInt32(dataRow[1]);

                    if (!furniIdAllow.Contains(baseID))
                    {
                        continue;
                    }

                    ButterflyEnvironment.GetGame().GetItemManager().GetItem(baseID, out ItemData Data);
                    if (Data == null || Data.IsRare)
                    {
                        continue;
                    }

                    int ItemId = ItemDao.InsertDuplicate(dbClient, Session.GetHabbo().Id, RoomId, OldItemId);

                    newItemsId.Add(OldItemId, ItemId);

                    if (Data.InteractionType == InteractionType.TELEPORT || Data.InteractionType == InteractionType.ARROW)
                    {
                        teleportId.Add(OldItemId);
                    }

                    if (Data.InteractionType == InteractionType.MOODLIGHT)
                    {
                        ItemMoodlightDao.InsertDuplicate(dbClient, ItemId, OldItemId);
                    }

                    if (WiredUtillity.TypeIsWired(Data.InteractionType))
                    {
                        ItemWiredDao.InsertDuplicate(dbClient, ItemId, OldItemId);

                        wiredId.Add(ItemId);
                    }
                }

                foreach (int oldId in teleportId)
                {
                    if (!newItemsId.TryGetValue(oldId, out int newId))
                    {
                        continue;
                    }

                    DataRow rowTele = ItemTeleportDao.GetOne(dbClient, oldId);
                    if (rowTele == null)
                    {
                        continue;
                    }

                    if (!newItemsId.TryGetValue(Convert.ToInt32(rowTele["tele_two_id"]), out int newIdTwo))
                    {
                        continue;
                    }

                    ItemTeleportDao.Insert(dbClient, newId, newIdTwo);
                }

                foreach (int id in wiredId)
                {
                    DataRow wiredRow = ItemWiredDao.GetOne(dbClient, id);

                    if (wiredRow == null)
                    {
                        continue;
                    }

                    string triggerItems = "";

                    string OldItem = (string)wiredRow["triggers_item"];

                    if (OldItem.Contains(":"))
                    {
                        foreach (string oldItem in OldItem.Split(';'))
                        {
                            string[] itemData = oldItem.Split(':');

                            if (itemData.Length != 6)
                            {
                                continue;
                            }

                            int oldId = Convert.ToInt32(itemData[0]);

                            if (!newItemsId.TryGetValue(Convert.ToInt32(oldId), out int newId))
                            {
                                continue;
                            }

                            triggerItems += newId + ":" + Convert.ToInt32(itemData[1]) + ":" + Convert.ToInt32(itemData[2]) + ":" + Convert.ToDouble(itemData[3]) + ":" + Convert.ToInt32(itemData[4]) + ":" + itemData[5] + ";";
                        }
                    }
                    else
                    {
                        foreach (string oldId in OldItem.Split(';'))
                        {
                            if (!newItemsId.TryGetValue(Convert.ToInt32(oldId), out int newId))
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

                BotDao.DupliqueAllBotInRoomId(dbClient, Session.GetHabbo().Id , RoomId, OldRoomId);
                
                PetDao.InsertDuplicate(dbClient, Session.GetHabbo().Id, RoomId, OldRoomId);
            }

            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (roomData == null)
            {
                return;
            }

            Session.GetHabbo().UsersRooms.Add(roomData);

            Session.SendPacket(new FlatCreatedComposer(roomData.Id, "Appart " + OldRoomId + " copie"));
        }
    }
}