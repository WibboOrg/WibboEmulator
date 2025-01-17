namespace WibboEmulator.Games.Users.Inventory;
using System.Collections.Concurrent;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core.Language;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Users.Inventory.Bots;
using WibboEmulator.Utilities;

public class InventoryComponent(User user) : IDisposable
{
    private readonly ConcurrentDictionary<int, Item> _userItems = new();
    private readonly ConcurrentDictionary<int, Pet> _petItems = new();
    private readonly ConcurrentDictionary<int, Bot> _botItems = new();
    private readonly int _furniLimit = SettingsManager.GetData<int>("inventory.furni.limit");
    private readonly int _petLimit = SettingsManager.GetData<int>("inventory.pet.limit");
    private readonly int _botLimit = SettingsManager.GetData<int>("inventory.bot.limit");
    private int _inventoryPoints;
    private bool _inventoryDefined;

    public void ClearItems(bool all = false)
    {
        if (all)
        {
            using var dbClient = DatabaseManager.Connection;

            ItemDao.DeleteAll(dbClient, user.Id);

            var rareAmounts = new Dictionary<int, int>();
            foreach (var roomItem in this.GetWallAndFloor)
            {
                if (roomItem == null || roomItem.ItemData == null || roomItem.ItemData.Amount < 0)
                {
                    continue;
                }

                roomItem.Data.Amount -= 1;

                if (!rareAmounts.TryGetValue(roomItem.BaseItemId, out var value))
                {
                    rareAmounts.Add(roomItem.BaseItemId, 1);
                }
                else
                {
                    _ = rareAmounts.Remove(roomItem.BaseItemId);
                    rareAmounts.Add(roomItem.BaseItemId, value + 1);
                }
            }

            ItemStatDao.UpdateRemove(dbClient, rareAmounts);

            this._userItems.Clear();
            this._inventoryPoints = 0;
        }
        else
        {
            using var dbClient = DatabaseManager.Connection;

            ItemDao.DeleteAllWithoutRare(dbClient, user.Id);

            this._userItems.Clear();

            var itemList = ItemDao.GetAllByUserId(dbClient, user.Id, this._furniLimit);

            foreach (var item in itemList)
            {
                var id = item.Id;
                var baseItem = item.BaseItem;
                var extraData = item.ExtraData;
                var limited = item.LimitedNumber ?? 0;
                var limitedTo = item.LimitedStack ?? 0;

                var userItem = new Item(id, 0, baseItem, extraData, limited, limitedTo, 0, 0, 0.0, 0, "", null);
                _ = this._userItems.TryAdd(id, userItem);
            }
        }

        user.Client.SendPacket(new FurniListUpdateComposer());
    }

    public List<Item> GetItemsByType(int baseItemId, int amount = 10000) => this.GetWallAndFloor.Where(x => x.BaseItemId == baseItemId).Take(amount).ToList();

    public int GetInventoryPoints() => this._inventoryPoints;

    public void DeleteItems(IDbConnection dbClient, List<Item> items, int baseItem = 0)
    {
        var listMessage = new ServerPacketList();
        var deleteItem = new List<Item>();
        var rareAmounts = new Dictionary<int, int>();

        foreach (var roomItem in items)
        {
            if (roomItem == null || roomItem.ItemData == null)
            {
                continue;
            }

            if (roomItem.ItemData.Amount > 0)
            {
                roomItem.Data.Amount -= 1;

                if (!rareAmounts.TryGetValue(roomItem.BaseItemId, out var value))
                {
                    rareAmounts.Add(roomItem.BaseItemId, 1);
                }
                else
                {
                    _ = rareAmounts.Remove(roomItem.BaseItemId);
                    rareAmounts.Add(roomItem.BaseItemId, value + 1);
                }
            }

            listMessage.Add(new FurniListRemoveComposer(roomItem.Id));
            deleteItem.Add(roomItem);
        }

        foreach (var roomItem in deleteItem)
        {
            this.RemoveItem(roomItem.Id, false);
        }

        if (baseItem > 0)
        {
            ItemDao.DeleteAllByRoomIdAndBaseItem(dbClient, 0, user.Id, baseItem);
        }
        else
        {
            ItemDao.DeleteItems(dbClient, deleteItem);
        }

        ItemStatDao.UpdateRemove(dbClient, rareAmounts);

        user.Client.SendPacket(listMessage);
    }

    public void ConvertMagot()
    {
        var wibboPointsCount = 0;
        var creditCount = 0;
        var winwinCount = 0;

        var deleteItem = new List<Item>();

        foreach (var roomItem in this.GetWallAndFloor)
        {
            if (roomItem == null || roomItem.ItemData == null || roomItem.ItemData.InteractionType != InteractionType.EXCHANGE)
            {
                continue;
            }

            var itemName = roomItem.ItemData.ItemName;

            var isParsed = itemName.StartsWith("nft_credit_")
                ? int.TryParse(itemName.Split("nft_credit_")[1], out var magotCount)
                : int.TryParse(itemName.Split('_')[1], out magotCount);

            if (!isParsed || magotCount <= 0)
            {
                continue;
            }

            if (itemName.StartsWith("CF_") || itemName.StartsWith("CFC_") || itemName.StartsWith("nft_credit_"))
            {
                creditCount += magotCount;
            }
            else if (itemName.StartsWith("PntEx_"))
            {
                wibboPointsCount += magotCount;
            }
            else if (itemName.StartsWith("WwnEx_"))
            {
                winwinCount += magotCount;
            }
            else
            {
                continue;
            }

            deleteItem.Add(roomItem);
        }

        if (deleteItem.Count == 0)
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        this.DeleteItems(dbClient, deleteItem);

        if (creditCount > 0)
        {
            user.Credits += creditCount;
            user.Client.SendPacket(new CreditBalanceComposer(user.Credits));
        }

        if (wibboPointsCount > 0)
        {
            this._inventoryPoints = 0;
            user.WibboPoints += wibboPointsCount;
            user.Client.SendPacket(new ActivityPointNotificationComposer(user.WibboPoints, 0, 105));

            UserDao.UpdateAddPoints(dbClient, user.Id, wibboPointsCount);
        }

        if (winwinCount > 0)
        {
            UserStatsDao.UpdateAchievementScore(dbClient, user.Id, winwinCount);

            user.AchievementPoints += winwinCount;
            user.Client.SendPacket(new AchievementScoreComposer(user.AchievementPoints));

            var room = user.Room;

            if (room != null)
            {
                var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(user.Id);
                if (roomUserByUserId != null)
                {
                    user.Client.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                    room.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                }
            }
        }
    }

    public void ClearPets()
    {
        using (var dbClient = DatabaseManager.Connection)
        {
            BotPetDao.Delete(dbClient, user.Id);
        }

        this._petItems.Clear();
    }

    public void ClearBots()
    {
        using (var dbClient = DatabaseManager.Connection)
        {
            BotUserDao.Delete(dbClient, user.Id);
        }

        this._botItems.Clear();
    }

    public void Dispose()
    {
        this._userItems.Clear();
        this._petItems.Clear();
        this._botItems.Clear();

        GC.SuppressFinalize(this);
    }

    public ICollection<Pet> Pets => this._petItems.Values;

    public bool TryAddPet(Pet pet)
    {
        pet.RoomId = 0;
        pet.PlacedInRoom = false;

        return this._petItems.TryAdd(pet.PetId, pet);
    }

    public bool TryRemovePet(int petId, out Pet petItem)
    {
        if (this._petItems.ContainsKey(petId))
        {
            return this._petItems.TryRemove(petId, out petItem);
        }
        else
        {
            petItem = null;
            return false;
        }
    }

    public bool TryGetPet(int petId, out Pet pet)
    {
        if (this._petItems.ContainsKey(petId))
        {
            return this._petItems.TryGetValue(petId, out pet);
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

    public ICollection<Bot> Bots => this._botItems.Values;

    public void TryAddItem(Item item)
    {
        if (this._userItems.TryAdd(item.Id, item))
        {
            this.AddInventoryPoint(item);

            user.Client.SendPacket(new UnseenItemsComposer(item.Id, UnseenItemsType.Furni));
            user.Client.SendPacket(new FurniListAddComposer(item));
        }
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
        this._petItems.Clear();

        using var dbClient = DatabaseManager.Connection;
        var itemList = ItemDao.GetAllByUserId(dbClient, user.Id, this._furniLimit);

        foreach (var item in itemList)
        {
            var id = item.Id;
            var baseItem = item.BaseItem;
            var extraData = item.ExtraData;
            var limited = item.LimitedNumber ?? 0;
            var limitedTo = item.LimitedStack ?? 0;

            var userItem = new Item(id, 0, baseItem, extraData, limited, limitedTo, 0, 0, 0.0, 0, "", null);
            _ = this._userItems.TryAdd(id, userItem);

            this.AddInventoryPoint(userItem);
        }

        if (this._userItems.Count >= this._furniLimit)
        {
            user.Client.SendHugeNotification(string.Format(LanguageManager.TryGetValue("inventory.limit.furni", user.Client.Language), this._furniLimit));
        }

        var botPetList = BotPetDao.GetAllByUserId(dbClient, user.Id, this._petLimit);
        if (botPetList.Count != 0)
        {
            foreach (var botPet in botPetList)
            {
                var pet = new Pet(botPet.Id, botPet.UserId, botPet.RoomId, botPet.Name, botPet.Type, botPet.Race, botPet.Color, botPet.Experience,
                botPet.Energy, botPet.Nutrition, botPet.Respect, botPet.CreateStamp, botPet.X, botPet.Y, botPet.Z, botPet.HaveSaddle, botPet.HairDye,
                botPet.PetHair, botPet.AnyoneRide);
                _ = this._petItems.TryAdd(pet.PetId, pet);
            }
        }

        if (this._petItems.Count >= this._petLimit)
        {
            user.Client.SendHugeNotification(string.Format(LanguageManager.TryGetValue("inventory.limit.pet", user.Client.Language), this._petLimit));
        }

        var botUserList = BotUserDao.GetAllByUserId(dbClient, user.Id, this._botLimit);
        if (botUserList.Count != 0)
        {
            foreach (var botUser in botUserList)
            {
                var bot = new Bot(botUser.Id, botUser.UserId, botUser.Name, botUser.Motto, botUser.Look, botUser.Gender, botUser.WalkEnabled,
                botUser.ChatEnabled, botUser.ChatText, botUser.ChatSeconds, botUser.IsDancing, botUser.Enable, botUser.HandItem, botUser.Status,
                BotUtility.GetAIFromString(botUser.AiType));
                _ = this._botItems.TryAdd(botUser.Id, bot);
            }
        }

        if (this._botItems.Count >= this._botLimit)
        {
            user.Client.SendHugeNotification(string.Format(LanguageManager.TryGetValue("inventory.limit.bot", user.Client.Language), this._botLimit));
        }
    }

    public void AddInventoryPoint(Item roomItem)
    {
        if (roomItem == null || roomItem.ItemData == null || roomItem.ItemData.InteractionType != InteractionType.EXCHANGE)
        {
            return;
        }

        if (!roomItem.ItemData.ItemName.StartsWith("PntEx_"))
        {
            return;
        }

        var magotCount = int.Parse(roomItem.ItemData.ItemName.Split('_')[1]);

        this._inventoryPoints += magotCount;
    }

    public void RemoveInventoryPoint(Item roomItem)
    {
        if (roomItem == null || roomItem.ItemData == null || roomItem.ItemData.InteractionType != InteractionType.EXCHANGE)
        {
            return;
        }

        if (!roomItem.ItemData.ItemName.StartsWith("PntEx_"))
        {
            return;
        }

        if (int.TryParse(roomItem.ItemData.ItemName.Split('_')[1], out var magotCount))
        {
            this._inventoryPoints -= magotCount;
        }
    }

    public Item GetItem(int id)
    {
        if (this._userItems.TryGetValue(id, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    private bool UserHoldsItem(int itemID) => this._userItems.ContainsKey(itemID);

    public void RemoveItem(int id, bool sendComposed = true)
    {
        if (this._userItems.TryGetValue(id, out var item))
        {
            this.RemoveInventoryPoint(item);

            _ = this._userItems.TryRemove(id, out _);
        }

        if (sendComposed)
        {
            user.Client.SendPacket(new FurniListRemoveComposer(id));
        }
    }

    public IEnumerable<Item> GetWallAndFloor => this._userItems.Values;

    public void AddItemArray(List<Item> roomItemList)
    {
        using var dbClient = DatabaseManager.Connection;
        foreach (var roomItem in roomItemList)
        {
            this.AddItem(dbClient, roomItem);
        }
    }

    public void AddItem(IDbConnection dbClient, Item item)
    {
        ItemDao.UpdateRoomIdAndUserId(dbClient, item.Id, 0, user.Id);

        var userItem = new Item(item.Id, 0, item.BaseItemId, item.ExtraData, item.Limited, item.LimitedStack, 0, 0, 0.0, 0, "", null);
        if (this.UserHoldsItem(item.Id))
        {
            this.RemoveItem(item.Id);
        }

        _ = this._userItems.TryAdd(userItem.Id, userItem);

        this.AddInventoryPoint(userItem);

        user.Client.SendPacket(new FurniListAddComposer(userItem));
    }

    public bool IsOverlowLimit(int amountPurchase, ItemType type)
    {
        if (type is ItemType.S or ItemType.I)
        {
            return this._userItems.Count + amountPurchase >= this._furniLimit;
        }
        else if (type is ItemType.R)
        {
            return this._botItems.Count + amountPurchase >= this._botLimit;
        }
        else if (type is ItemType.P)
        {
            return this._petItems.Count + amountPurchase >= this._petLimit;
        }

        return false;
    }
}
