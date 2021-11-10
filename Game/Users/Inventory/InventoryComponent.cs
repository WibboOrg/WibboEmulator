using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Pets;
using Butterfly.Game.Users.Inventory.Bots;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Users.Inventory
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
                    ItemDao.DeleteAll(dbClient, this.UserId);
                }

                this._UserItems.Clear();
            }
            else
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.DeleteAllWithoutRare(dbClient, this.UserId);
                }

                this._UserItems.Clear();
                DataTable table1;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    table1 = ItemDao.GetAllByUserId(dbClient, this.UserId);
                }

                foreach (DataRow dataRow in table1.Rows)
                {
                    int Id = Convert.ToInt32(dataRow["id"]);
                    int BaseItem = Convert.ToInt32(dataRow["base_item"]);
                    string ExtraData = DBNull.Value.Equals(dataRow["extra_data"]) ? string.Empty : (string)dataRow["extra_data"];
                    int Limited = DBNull.Value.Equals(dataRow["limited_number"]) ? 0 : Convert.ToInt32(dataRow["limited_number"]);
                    int LimitedTo = DBNull.Value.Equals(dataRow["limited_stack"]) ? 0 : Convert.ToInt32(dataRow["limited_stack"]);

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
                PetDao.Delete(dbClient, this.UserId);
            }

            this._petsItems.Clear();
        }

        public void ClearBots()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                BotDao.Delete(dbClient, this.UserId);

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
            DataTable table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                table = ItemDao.GetAllByUserId(dbClient, this.UserId);
            }

            foreach (DataRow dataRow in table.Rows)
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
                table2 = PetDao.GetAllByUserId(dbClient, this.UserId);
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
                dBots = BotDao.GetAllByUserId(dbClient, this.UserId);

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
                ItemDao.UpdateRoomIdAndUserId(dbClient, Id, 0, this.UserId);
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
                ItemDao.UpdateRoomIdAndUserId(dbClient, item.Id, 0, this.UserId);
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