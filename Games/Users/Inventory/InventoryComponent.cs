using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Pets;
using WibboEmulator.Games.GameClients.Inventory.Bots;
using System.Collections.Concurrent;
using System.Data;

namespace WibboEmulator.Games.GameClients.Inventory
{
    public class InventoryComponent : IDisposable
    {
        private readonly ConcurrentDictionary<int, Item> _userItems;
        private readonly ConcurrentDictionary<int, Pet> _petsItems;
        private readonly ConcurrentDictionary<int, Bot> _botItems;

        private readonly User _userInstance;
        private bool _inventoryDefined;

        public InventoryComponent(User user)
        {
            this._userInstance = user;

            this._userItems = new ConcurrentDictionary<int, Item>();
            this._petsItems = new ConcurrentDictionary<int, Pet>();
            this._botItems = new ConcurrentDictionary<int, Bot>();
        }

        public void ClearItems(bool All = false)
        {
            if (All)
            {
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

                ItemDao.DeleteAll(dbClient, this._userInstance.Id);

                Dictionary<int, int> rareAmounts = new Dictionary<int, int>();
                foreach (Item roomItem in this.GetWallAndFloor)
                {
                    if (roomItem == null || roomItem.GetBaseItem() == null || roomItem.GetBaseItem().Amount < 0)
                        continue;

                    roomItem.Data.Amount -= 1;

                    if (!rareAmounts.TryGetValue(roomItem.BaseItem, out int value))
                        rareAmounts.Add(roomItem.BaseItem, 1);
                    else
                    {
                        rareAmounts.Remove(roomItem.BaseItem);
                        rareAmounts.Add(roomItem.BaseItem, value + 1);
                    }
                }

                ItemStatDao.UpdateRemove(dbClient, rareAmounts);

                this._userItems.Clear();
            }
            else
            {
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

                ItemDao.DeleteAllWithoutRare(dbClient, this._userInstance.Id);

                this._userItems.Clear();

                DataTable table = ItemDao.GetAllByUserId(dbClient, this._userInstance.Id);

                foreach (DataRow dataRow in table.Rows)
                {
                    int Id = Convert.ToInt32(dataRow["id"]);
                    int BaseItem = Convert.ToInt32(dataRow["base_item"]);
                    string ExtraData = DBNull.Value.Equals(dataRow["extra_data"]) ? string.Empty : (string)dataRow["extra_data"];
                    int Limited = DBNull.Value.Equals(dataRow["limited_number"]) ? 0 : Convert.ToInt32(dataRow["limited_number"]);
                    int LimitedTo = DBNull.Value.Equals(dataRow["limited_stack"]) ? 0 : Convert.ToInt32(dataRow["limited_stack"]);

                    Item userItem = new Item(Id, 0, BaseItem, ExtraData, Limited, LimitedTo, 0, 0, 0.0, 0, "", null);
                    this._userItems.TryAdd(Id, userItem);
                }
            }

            this.GetClient().SendPacket(new FurniListUpdateComposer());
        }

        public void ClearPets()
        {
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.Delete(dbClient, this._userInstance.Id);
            }

            this._petsItems.Clear();
        }

        public void ClearBots()
        {
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                BotUserDao.Delete(dbClient, this._userInstance.Id);

            this._botItems.Clear();
        }

        public void Dispose()
        {
            this._userItems.Clear();
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
            this._userInstance.GetClient().SendPacket(new FurniListAddComposer(item));

            return this._userItems.TryAdd(item.Id, item);
        }

        public void LoadInventory()
        {
            if (this._inventoryDefined)
                return;

            this._inventoryDefined = true;

            this._userItems.Clear();
            this._botItems.Clear();
            this._petsItems.Clear();

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            DataTable dItems = ItemDao.GetAllByUserId(dbClient, this._userInstance.Id);

            foreach (DataRow dataRow in dItems.Rows)
            {
                int Id = Convert.ToInt32(dataRow["id"]);
                int BaseItem = Convert.ToInt32(dataRow["base_item"]);
                string ExtraData = DBNull.Value.Equals(dataRow["extra_data"]) ? string.Empty : (string)dataRow["extra_data"];
                int Limited = DBNull.Value.Equals(dataRow["limited_number"]) ? 0 : Convert.ToInt32(dataRow["limited_number"]);
                int LimitedTo = DBNull.Value.Equals(dataRow["limited_stack"]) ? 0 : Convert.ToInt32(dataRow["limited_stack"]);

                Item userItem = new Item(Id, 0, BaseItem, ExtraData, Limited, LimitedTo, 0, 0, 0.0, 0, "", null);
                this._userItems.TryAdd(Id, userItem);
            }

            DataTable dPets = BotPetDao.GetAllByUserId(dbClient, this._userInstance.Id);
            if (dPets != null)
            {
                foreach (DataRow Row in dPets.Rows)
                {
                    Pet pet = new Pet(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["room_id"]), (string)Row["name"], Convert.ToInt32(Row["type"]), (string)Row["race"], (string)Row["color"], Convert.ToInt32(Row["experience"]), Convert.ToInt32(Row["energy"]), Convert.ToInt32(Row["nutrition"]), Convert.ToInt32(Row["respect"]), (double)Row["createstamp"], Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]), (double)Row["z"], Convert.ToInt32(Row["have_saddle"]), Convert.ToInt32(Row["hairdye"]), Convert.ToInt32(Row["pethair"]), (string)(Row["anyone_ride"]) == "1");
                    this._petsItems.TryAdd(pet.PetId, pet);
                }
            }

            DataTable dBots = BotUserDao.GetAllByUserId(dbClient, this._userInstance.Id);
            if (dBots != null)
            {
                foreach (DataRow Row in dBots.Rows)
                {
                    Bot bot = new Bot(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), (string)Row["name"], (string)Row["motto"], (string)Row["look"], (string)Row["gender"], (string)Row["walk_enabled"] == "1", (string)Row["chat_enabled"] == "1", (string)Row["chat_text"], Convert.ToInt32(Row["chat_seconds"]), (string)Row["is_dancing"] == "1", Convert.ToInt32(Row["enable"]), Convert.ToInt32(Row["handitem"]), Convert.ToInt32((string)Row["status"]));
                    this._botItems.TryAdd(Convert.ToInt32(Row["id"]), bot);
                }
            }
        }

        public Item GetItem(int Id)
        {
            if (this._userItems.ContainsKey(Id))
            {
                return this._userItems[Id];
            }
            else
            {
                return null;
            }
        }

        public Item AddNewItem(int Id, int BaseItem, string ExtraData, int Limited = 0, int LimitedStack = 0)
        {
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateRoomIdAndUserId(dbClient, Id, 0, this._userInstance.Id);
            }

            Item userItem = new Item(Id, 0, BaseItem, ExtraData, Limited, LimitedStack, 0, 0, 0.0, 0, "", null);
            if (this.UserHoldsItem(Id))
            {
                this.RemoveItem(Id);
            }

            this._userItems.TryAdd(userItem.Id, userItem);

            this._userInstance.GetClient().SendPacket(new FurniListAddComposer(userItem));

            return userItem;
        }

        private bool UserHoldsItem(int itemID)
        {
            return this._userItems.ContainsKey(itemID);
        }

        public void RemoveItem(int Id)
        {
            if (this._userItems.ContainsKey(Id))
            {
                this._userItems.TryRemove(Id, out Item ToRemove);
            }

            this.GetClient().SendPacket(new FurniListRemoveComposer(Id));
        }

        public IEnumerable<Item> GetWallAndFloor => this._userItems.Values;

        private GameClient GetClient()
        {
            return WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this._userInstance.Id);
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
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateRoomIdAndUserId(dbClient, item.Id, 0, this._userInstance.Id);
            }

            Item userItem = new Item(item.Id, 0, item.BaseItem, item.ExtraData, item.Limited, item.LimitedStack, 0, 0, 0.0, 0, "", null);
            if (this.UserHoldsItem(item.Id))
            {
                this.RemoveItem(item.Id);
            }

            this._userItems.TryAdd(userItem.Id, userItem);

            this._userInstance.GetClient().SendPacket(new FurniListAddComposer(userItem));
        }
    }
}