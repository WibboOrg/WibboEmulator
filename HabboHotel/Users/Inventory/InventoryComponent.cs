using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Pets;
using Butterfly.HabboHotel.Users.Inventory.Bots;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Users.Inventory
{
    public class InventoryComponent
    {
        private readonly ConcurrentDictionary<int, Item> _UserItems;
        private readonly ConcurrentDictionary<int, Pet> _petsItems;
        private readonly ConcurrentDictionary<int, Bot> _botItems;

        private GameClient _client;

        public bool inventoryDefined;
        public int UserId;

        public InventoryComponent(int UserId, GameClient Client)
        {
            this._client = Client;
            this.UserId = UserId;

            this._UserItems = new ConcurrentDictionary<int, Item>();
            this._petsItems = new ConcurrentDictionary<int, Pet>();
            this._botItems = new ConcurrentDictionary<int, Bot>();
        }

        public void ClearItems(bool All = false)
        {
            if (All)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE items, items_limited, user_presents, room_items_moodlight, tele_links, wired_items FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '0' AND user_id = '" + this.UserId + "'");
                }

                this._UserItems.Clear();
            }
            else
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE items, items_limited, user_presents, room_items_moodlight, tele_links, wired_items FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '0' AND user_id = '" + this.UserId + "' AND base_item NOT IN (SELECT id FROM furniture WHERE is_rare = '1')");
                }

                this._UserItems.Clear();
                DataTable table1;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT items.id, items.base_item, items.extra_data, items_limited.limited_number, items_limited.limited_stack FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.user_id = @userid AND items.room_id = '0'");

                    dbClient.AddParameter("userid", this.UserId);
                    table1 = dbClient.GetTable();
                }

                foreach (DataRow dataRow in table1.Rows)
                {
                    int Id = Convert.ToInt32(dataRow[0]);
                    int BaseItem = Convert.ToInt32(dataRow[1]);
                    string ExtraData = DBNull.Value.Equals(dataRow[2]) ? string.Empty : (string)dataRow[2];
                    int Limited = DBNull.Value.Equals(dataRow[3]) ? 0 : Convert.ToInt32(dataRow[3]);
                    int LimitedTo = DBNull.Value.Equals(dataRow[4]) ? 0 : Convert.ToInt32(dataRow[4]);

                    Item userItem = new Item(Id, 0, BaseItem, ExtraData, Limited, LimitedTo, 0, 0, 0.0, 0, "", null);
                    this._UserItems.TryAdd(Id, userItem);
                }
            }

            this.GetClient().SendPacket(new FurniListUpdateComposer());
        }

        public void ClearPets()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM pets WHERE room_id = '0' AND user_id = '" + this.UserId + "'");
            }

            this._petsItems.Clear();
        }

        public void ClearBots()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM bots WHERE room_id = '0' AND user_id = '" + this.UserId + "'");
            }

            this._botItems.Clear();
        }

        public void SetActiveState(GameClient client)
        {
            this._client = client;
        }

        public void Destroy()
        {
            this._client = null;
            this._UserItems.Clear();
            this._petsItems.Clear();
            this._botItems.Clear();
        }

        public ICollection<Pet> GetPets()
        {
            return this._petsItems.Values;
        }

        public bool TryAddPet(Pet Pet)
        {
            Pet.RoomId = 0;
            Pet.PlacedInRoom = false;

            return this._petsItems.TryAdd(Pet.PetId, Pet);
        }

        public bool TryRemovePet(int PetId, out Pet PetItem)
        {
            if (this._petsItems.ContainsKey(PetId))
            {
                return this._petsItems.TryRemove(PetId, out PetItem);
            }
            else
            {
                PetItem = null;
                return false;
            }
        }

        public bool TryGetPet(int PetId, out Pet Pet)
        {
            if (this._petsItems.ContainsKey(PetId))
            {
                return this._petsItems.TryGetValue(PetId, out Pet);
            }
            else
            {
                Pet = null;
                return false;
            }
        }

        public bool TryGetBot(int BotId, out Bot Bot)
        {
            if (this._botItems.ContainsKey(BotId))
            {
                return this._botItems.TryGetValue(BotId, out Bot);
            }
            else
            {
                Bot = null;
                return false;
            }
        }

        public bool TryRemoveBot(int BotId, out Bot Bot)
        {
            if (this._botItems.ContainsKey(BotId))
            {
                return this._botItems.TryRemove(BotId, out Bot);
            }
            else
            {
                Bot = null;
                return false;
            }
        }

        public bool TryAddBot(Bot Bot)
        {
            return this._botItems.TryAdd(Bot.Id, Bot);
        }

        public ICollection<Bot> GetBots()
        {
            return this._botItems.Values;
        }

        public bool TryAddItem(Item item)
        {
            if (this._client != null)
            {
                this._client.SendPacket(new FurniListAddComposer(item));
            }

            return this._UserItems.TryAdd(item.Id, item);
        }

        public void LoadInventory()
        {
            this._UserItems.Clear();
            DataTable table1;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT items.id, items.base_item, items.extra_data, items_limited.limited_number, items_limited.limited_stack FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.user_id = @userid AND items.room_id = '0'");

                dbClient.AddParameter("userid", this.UserId);
                table1 = dbClient.GetTable();
            }

            foreach (DataRow dataRow in table1.Rows)
            {
                int Id = Convert.ToInt32(dataRow[0]);
                int BaseItem = Convert.ToInt32(dataRow[1]);
                string ExtraData = DBNull.Value.Equals(dataRow[2]) ? string.Empty : (string)dataRow[2];
                int Limited = DBNull.Value.Equals(dataRow[3]) ? 0 : Convert.ToInt32(dataRow[3]);
                int LimitedTo = DBNull.Value.Equals(dataRow[4]) ? 0 : Convert.ToInt32(dataRow[4]);

                Item userItem = new Item(Id, 0, BaseItem, ExtraData, Limited, LimitedTo, 0, 0, 0.0, 0, "", null);
                this._UserItems.TryAdd(Id, userItem);
            }

            this._petsItems.Clear();
            DataTable table2;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id, user_id, room_id, name, type, race, color, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM pets WHERE user_id = '" + this.UserId + "' AND room_id = 0");
                table2 = dbClient.GetTable();
            }
            if (table2 != null)
            {
                foreach (DataRow Row in table2.Rows)
                {
                    Pet pet = new Pet(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["room_id"]), (string)Row["name"], Convert.ToInt32(Row["type"]), (string)Row["race"], (string)Row["color"], Convert.ToInt32(Row["experience"]), Convert.ToInt32(Row["energy"]), Convert.ToInt32(Row["nutrition"]), Convert.ToInt32(Row["respect"]), (double)Row["createstamp"], Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]), (double)Row["z"], Convert.ToInt32(Row["have_saddle"]), Convert.ToInt32(Row["hairdye"]), Convert.ToInt32(Row["pethair"]), (string)(Row["anyone_ride"]) == "1");
                    this._petsItems.TryAdd(pet.PetId, pet);
                }
            }

            this._botItems.Clear();
            DataTable dBots;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM bots WHERE user_id = '" + this.UserId + "' AND room_id = '0'");
                dBots = dbClient.GetTable();
            }
            if (dBots == null)
            {
                return;
            }

            foreach (DataRow Row in dBots.Rows)
            {
                this._botItems.TryAdd(Convert.ToInt32(Row["id"]), new Bot(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), (string)Row["name"], (string)Row["motto"], (string)Row["look"], (string)Row["gender"], (string)Row["walk_enabled"] == "1", (string)Row["chat_enabled"] == "1", (string)Row["chat_text"], Convert.ToInt32(Row["chat_seconds"]), (string)Row["is_dancing"] == "1", Convert.ToInt32(Row["enable"]), Convert.ToInt32(Row["handitem"]), Convert.ToInt32((string)Row["status"])));
            }
        }


        public int getFloorInventoryAmount()
        {
            return this._UserItems.Count;
        }

        public Item GetItem(int Id)
        {
            if (this._UserItems.ContainsKey(Id))
            {
                return this._UserItems[Id];
            }
            else
            {
                return null;
            }
        }

        public Item AddNewItem(int Id, int BaseItem, string ExtraData, int Limited = 0, int LimitedStack = 0)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE items SET room_id = '0', user_id = '" + this.UserId + "' WHERE id = '" + Id + "'");
            }

            Item userItem = new Item(Id, 0, BaseItem, ExtraData, Limited, LimitedStack, 0, 0, 0.0, 0, "", null);
            if (this.UserHoldsItem(Id))
            {
                this.RemoveItem(Id);
            }

            this._UserItems.TryAdd(userItem.Id, userItem);

            if (this._client != null)
            {
                this._client.SendPacket(new FurniListAddComposer(userItem));
            }

            return userItem;
        }

        private bool UserHoldsItem(int itemID)
        {
            return this._UserItems.ContainsKey(itemID);
        }

        public void RemoveItem(int Id)
        {
            if (this._UserItems.ContainsKey(Id))
            {
                this._UserItems.TryRemove(Id, out Item ToRemove);
            }

            this.GetClient().SendPacket(new FurniListRemoveComposer(Id));
        }

        public IEnumerable<Item> GetWallAndFloor => this._UserItems.Values;

        private GameClient GetClient()
        {
            return ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.UserId);
        }

        public void AddItemArray(List<Item> RoomItemList)
        {
            foreach (Item roomItem in RoomItemList)
            {
                this.AddItem(roomItem);
            }
        }

        public void AddItem(Item item)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE items SET room_id = '0', user_id = '" + this.UserId + "' WHERE id = '" + item.Id + "'");
            }

            Item userItem = new Item(item.Id, 0, item.BaseItem, item.ExtraData, item.Limited, item.LimitedStack, 0, 0, 0.0, 0, "", null);
            if (this.UserHoldsItem(item.Id))
            {
                this.RemoveItem(item.Id);
            }

            this._UserItems.TryAdd(userItem.Id, userItem);

            if (this._client != null)
            {
                this._client.SendPacket(new FurniListAddComposer(userItem));
            }
        }
    }
}