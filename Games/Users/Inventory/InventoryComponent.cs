namespace WibboEmulator.Games.Users.Inventory;
using System.Collections.Concurrent;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Users.Inventory.Bots;

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

    public void ClearItems(bool all = false)
    {
        if (all)
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

            ItemDao.DeleteAll(dbClient, this._userInstance.Id);

            var rareAmounts = new Dictionary<int, int>();
            foreach (var roomItem in this.GetWallAndFloor)
            {
                if (roomItem == null || roomItem.GetBaseItem() == null || roomItem.GetBaseItem().Amount < 0)
                {
                    continue;
                }

                roomItem.Data.Amount -= 1;

                if (!rareAmounts.TryGetValue(roomItem.BaseItem, out var value))
                {
                    rareAmounts.Add(roomItem.BaseItem, 1);
                }
                else
                {
                    _ = rareAmounts.Remove(roomItem.BaseItem);
                    rareAmounts.Add(roomItem.BaseItem, value + 1);
                }
            }

            ItemStatDao.UpdateRemove(dbClient, rareAmounts);

            this._userItems.Clear();
        }
        else
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

            ItemDao.DeleteAllWithoutRare(dbClient, this._userInstance.Id);

            this._userItems.Clear();

            var table = ItemDao.GetAllByUserId(dbClient, this._userInstance.Id);

            foreach (DataRow dataRow in table.Rows)
            {
                var id = Convert.ToInt32(dataRow["id"]);
                var baseItem = Convert.ToInt32(dataRow["base_item"]);
                var extraData = DBNull.Value.Equals(dataRow["extra_data"]) ? string.Empty : (string)dataRow["extra_data"];
                var limited = DBNull.Value.Equals(dataRow["limited_number"]) ? 0 : Convert.ToInt32(dataRow["limited_number"]);
                var limitedTo = DBNull.Value.Equals(dataRow["limited_stack"]) ? 0 : Convert.ToInt32(dataRow["limited_stack"]);

                var userItem = new Item(id, 0, baseItem, extraData, limited, limitedTo, 0, 0, 0.0, 0, "", null);
                _ = this._userItems.TryAdd(id, userItem);
            }
        }

        this.GetClient().SendPacket(new FurniListUpdateComposer());
    }

    public void ClearPets()
    {
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotPetDao.Delete(dbClient, this._userInstance.Id);
        }

        this._petsItems.Clear();
    }

    public void ClearBots()
    {
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotUserDao.Delete(dbClient, this._userInstance.Id);
        }

        this._botItems.Clear();
    }

    public void Dispose()
    {
        this._userItems.Clear();
        this._petsItems.Clear();
        this._botItems.Clear();

        GC.SuppressFinalize(this);
    }

    public ICollection<Pet> GetPets() => this._petsItems.Values;

    public bool TryAddPet(Pet pet)
    {
        pet.RoomId = 0;
        pet.PlacedInRoom = false;

        return this._petsItems.TryAdd(pet.PetId, pet);
    }

    public bool TryRemovePet(int petId, out Pet petItem)
    {
        if (this._petsItems.ContainsKey(petId))
        {
            return this._petsItems.TryRemove(petId, out petItem);
        }
        else
        {
            petItem = null;
            return false;
        }
    }

    public bool TryGetPet(int petId, out Pet pet)
    {
        if (this._petsItems.ContainsKey(petId))
        {
            return this._petsItems.TryGetValue(petId, out pet);
        }
        else
        {
            pet = null;
            return false;
        }
    }

    public bool TryGetBot(int botId, out Bot bot)
    {
        if (this._botItems.ContainsKey(botId))
        {
            return this._botItems.TryGetValue(botId, out bot);
        }
        else
        {
            bot = null;
            return false;
        }
    }

    public bool TryRemoveBot(int botId, out Bot bot)
    {
        if (this._botItems.ContainsKey(botId))
        {
            return this._botItems.TryRemove(botId, out bot);
        }
        else
        {
            bot = null;
            return false;
        }
    }

    public bool TryAddBot(Bot bot) => this._botItems.TryAdd(bot.Id, bot);

    public ICollection<Bot> GetBots() => this._botItems.Values;

    public bool TryAddItem(Item item)
    {
        this._userInstance.Client.SendPacket(new FurniListAddComposer(item));

        return this._userItems.TryAdd(item.Id, item);
    }

    public void LoadInventory()
    {
        if (this._inventoryDefined)
        {
            return;
        }

        this._inventoryDefined = true;

        this._userItems.Clear();
        this._botItems.Clear();
        this._petsItems.Clear();

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        var dItems = ItemDao.GetAllByUserId(dbClient, this._userInstance.Id);

        foreach (DataRow dataRow in dItems.Rows)
        {
            var id = Convert.ToInt32(dataRow["id"]);
            var baseItem = Convert.ToInt32(dataRow["base_item"]);
            var extraData = DBNull.Value.Equals(dataRow["extra_data"]) ? string.Empty : (string)dataRow["extra_data"];
            var limited = DBNull.Value.Equals(dataRow["limited_number"]) ? 0 : Convert.ToInt32(dataRow["limited_number"]);
            var limitedTo = DBNull.Value.Equals(dataRow["limited_stack"]) ? 0 : Convert.ToInt32(dataRow["limited_stack"]);

            var userItem = new Item(id, 0, baseItem, extraData, limited, limitedTo, 0, 0, 0.0, 0, "", null);
            _ = this._userItems.TryAdd(id, userItem);
        }

        var dPets = BotPetDao.GetAllByUserId(dbClient, this._userInstance.Id);
        if (dPets != null)
        {
            foreach (DataRow row in dPets.Rows)
            {
                var pet = new Pet(Convert.ToInt32(row["id"]), Convert.ToInt32(row["user_id"]), Convert.ToInt32(row["room_id"]), (string)row["name"], Convert.ToInt32(row["type"]), (string)row["race"], (string)row["color"], Convert.ToInt32(row["experience"]), Convert.ToInt32(row["energy"]), Convert.ToInt32(row["nutrition"]), Convert.ToInt32(row["respect"]), (double)row["createstamp"], Convert.ToInt32(row["x"]), Convert.ToInt32(row["y"]), (double)row["z"], Convert.ToInt32(row["have_saddle"]), Convert.ToInt32(row["hairdye"]), Convert.ToInt32(row["pethair"]), (string)row["anyone_ride"] == "1");
                _ = this._petsItems.TryAdd(pet.PetId, pet);
            }
        }

        var dBots = BotUserDao.GetAllByUserId(dbClient, this._userInstance.Id);
        if (dBots != null)
        {
            foreach (DataRow row in dBots.Rows)
            {
                var bot = new Bot(Convert.ToInt32(row["id"]), Convert.ToInt32(row["user_id"]), (string)row["name"], (string)row["motto"], (string)row["look"], (string)row["gender"], (string)row["walk_enabled"] == "1", (string)row["chat_enabled"] == "1", (string)row["chat_text"], Convert.ToInt32(row["chat_seconds"]), (string)row["is_dancing"] == "1", Convert.ToInt32(row["enable"]), Convert.ToInt32(row["handitem"]), Convert.ToInt32((string)row["status"]));
                _ = this._botItems.TryAdd(Convert.ToInt32(row["id"]), bot);
            }
        }
    }

    public Item GetItem(int id)
    {
        if (this._userItems.ContainsKey(id))
        {
            return this._userItems[id];
        }
        else
        {
            return null;
        }
    }

    public Item AddNewItem(int id, int baseItem, string extraData, int limited = 0, int limitedStack = 0)
    {
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.UpdateRoomIdAndUserId(dbClient, id, 0, this._userInstance.Id);
        }

        var userItem = new Item(id, 0, baseItem, extraData, limited, limitedStack, 0, 0, 0.0, 0, "", null);
        if (this.UserHoldsItem(id))
        {
            this.RemoveItem(id);
        }

        _ = this._userItems.TryAdd(userItem.Id, userItem);

        this._userInstance.
        Client.SendPacket(new FurniListAddComposer(userItem));

        return userItem;
    }

    private bool UserHoldsItem(int itemID) => this._userItems.ContainsKey(itemID);

    public void RemoveItem(int id)
    {
        if (this._userItems.ContainsKey(id))
        {
            _ = this._userItems.TryRemove(id, out _);
        }

        this.GetClient().SendPacket(new FurniListRemoveComposer(id));
    }

    public IEnumerable<Item> GetWallAndFloor => this._userItems.Values;

    private GameClient GetClient() => WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this._userInstance.Id);

    public void AddItemArray(List<Item> roomItemList)
    {
        foreach (var roomItem in roomItemList)
        {
            this.AddItem(roomItem);
        }
    }

    public void AddItem(Item item)
    {
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.UpdateRoomIdAndUserId(dbClient, item.Id, 0, this._userInstance.Id);
        }

        var userItem = new Item(item.Id, 0, item.BaseItem, item.ExtraData, item.Limited, item.LimitedStack, 0, 0, 0.0, 0, "", null);
        if (this.UserHoldsItem(item.Id))
        {
            this.RemoveItem(item.Id);
        }

        _ = this._userItems.TryAdd(userItem.Id, userItem);

        this._userInstance.
        Client.SendPacket(new FurniListAddComposer(userItem));
    }
}
