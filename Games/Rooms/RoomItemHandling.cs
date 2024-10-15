namespace WibboEmulator.Games.Rooms;
using System.Collections.Concurrent;
using System.Data;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.Map.Movement;
using WibboEmulator.Games.Rooms.Moodlight;
using WibboEmulator.Utilities;

public class RoomItemHandling(Room room)
{
    private readonly ConcurrentDictionary<int, Item> _floorItems = new();
    private readonly ConcurrentDictionary<int, Item> _wallItems = new();
    private readonly ConcurrentDictionary<int, Item> _rollers = new();

    private readonly ConcurrentDictionary<int, ItemTemp> _itemsTemp = new();

    private readonly ConcurrentDictionary<int, Item> _updateItems = new();

    private readonly List<int> _rollerItemsMoved = [];
    private readonly List<int> _rollerUsersMoved = [];
    private readonly ServerPacketList _rollerMessages = new();

    private int _rollerSpeed = 4;
    private int _rollerCycle;
    private readonly ConcurrentQueue<Item> _roomItemUpdateQueue = new();
    private int _itemTempoId;

    public void QueueRoomItemUpdate(Item item) => this._roomItemUpdateQueue.Enqueue(item);

    public List<Item> RemoveAllFurnitureToInventory(GameClient Session)
    {
        var listMessage = new ServerPacketList();
        var items = new List<Item>();

        foreach (var roomItem in this._floorItems.Values.ToList())
        {
            roomItem.Interactor.OnRemove(Session, roomItem);

            roomItem.Destroy();
            listMessage.Add(new ObjectRemoveComposer(roomItem.Id, Session.User.Id));
            items.Add(roomItem);
        }

        foreach (var roomItem in this._wallItems.Values.ToList())
        {
            roomItem.Interactor.OnRemove(Session, roomItem);
            roomItem.Destroy();

            listMessage.Add(new ItemRemoveComposer(roomItem.Id, room.RoomData.OwnerId));
            items.Add(roomItem);
        }
        room.SendPackets(listMessage);

        this._wallItems.Clear();
        this._floorItems.Clear();
        this._itemsTemp.Clear();
        this._updateItems.Clear();
        this._rollers.Clear();
        using (var dbClient = DatabaseManager.Connection)
        {
            ItemDao.UpdateRoomIdAndUserId(dbClient, room.RoomData.OwnerId, room.Id);
        }

        room.GameMap.GenerateMaps();
        room.RoomUserManager.UpdateUserStatusses();
        room.WiredHandler.OnPickall();

        return items;
    }

    public List<Item> RemoveFurnitureToInventoryByIds(GameClient Session, List<int> itemIds)
    {
        var listMessage = new ServerPacketList();
        var items = new List<Item>();

        foreach (var itemId in itemIds)
        {
            var item = this.GetItem(itemId);
            if (item == null)
            {
                continue;
            }

            item.Interactor.OnRemove(Session, item);
            this.RemoveRoomItem(item);
            item.Destroy();

            items.Add(item);
            listMessage.Add(item.IsWallItem ? new ItemRemoveComposer(item.Id, room.RoomData.OwnerId) : new ObjectRemoveComposer(item.Id, room.RoomData.OwnerId));
        }

        using var dbClient = DatabaseManager.Connection;
        ItemDao.UpdateItems(dbClient, items, Session.User.Id);

        room.SendPackets(listMessage);

        return items;
    }

    public void SetSpeed(int p) => this._rollerSpeed = p;

    public void LoadFurniture(IDbConnection dbClient, int roomId = 0)
    {
        if (roomId == 0)
        {
            this._floorItems.Clear();
            this._wallItems.Clear();
        }

        int itemID;
        int userId;
        int baseID;
        string extraData;
        int x;
        int y;
        double z;
        int rot;
        string wallposs;
        int limited;
        int limitedTo;
        string wallCoord;

        string wiredTriggerData;
        string wiredTriggerData2;
        string wiredTriggersItem;
        bool wiredAllUserTriggerable;
        int wiredDelay;

        bool moodlightEnabled;
        int moodlightCurrentPreset;
        string moodlightPresetOne;
        string moodlightPresetTwo;
        string moodlightPresetThree;

        var itemList = ItemDao.GetAll(dbClient, (roomId == 0) ? room.Id : roomId);

        if (itemList.Count == 0)
        {
            return;
        }

        foreach (var item in itemList)
        {
            itemID = item.Id;
            userId = item.UserId;
            baseID = item.BaseItem;
            extraData = item.ExtraData;
            x = item.X;
            y = item.Y;
            z = item.Z;
            rot = item.Rot;
            wallposs = item.WallPos;
            limited = item.LimitedNumber ?? 0;
            limitedTo = item.LimitedStack ?? 0;

            if (!ItemManager.GetItem(baseID, out var data))
            {
                continue;
            }

            if (data.Type == ItemType.I)
            {
                if (string.IsNullOrEmpty(wallposs))
                {
                    wallCoord = "w=0,0 l=0,0 l";
                }
                else
                {
                    wallCoord = wallposs;
                }

                var roomItem = new Item(itemID, room.Id, baseID, extraData, limited, limitedTo, 0, 0, 0.0, 0, wallCoord, room);
                if (!this._wallItems.ContainsKey(itemID))
                {
                    _ = this._wallItems.TryAdd(itemID, roomItem);
                }

                if (roomItem.ItemData.InteractionType == InteractionType.MOODLIGHT)
                {
                    moodlightEnabled = item.Enabled;
                    moodlightCurrentPreset = item.CurrentPreset;
                    moodlightPresetOne = item.PresetOne;
                    moodlightPresetTwo = item.PresetTwo;
                    moodlightPresetThree = item.PresetThree;

                    room.MoodlightData = new MoodlightData(roomItem.Id, moodlightEnabled, moodlightCurrentPreset, moodlightPresetOne, moodlightPresetTwo, moodlightPresetThree);
                    roomItem.ExtraData = room.MoodlightData.GenerateExtraData();
                }
            }
            else //Is flooritem
            {
                var roomItem = new Item(itemID, room.Id, baseID, extraData, limited, limitedTo, x, y, z, rot, "", room);

                if (!this._floorItems.ContainsKey(itemID))
                {
                    _ = this._floorItems.TryAdd(itemID, roomItem);
                }

                if (WiredUtillity.TypeIsWired(data.InteractionType))
                {
                    wiredTriggerData = item.TriggerData ?? string.Empty;
                    wiredTriggerData2 = item.TriggerData2 ?? string.Empty;
                    wiredTriggersItem = item.TriggersItem ?? string.Empty;
                    wiredAllUserTriggerable = item.AllUserTriggerable;
                    wiredDelay = item.Delay;

                    WiredRegister.HandleRegister(roomItem, room, wiredTriggerData, wiredTriggerData2, wiredTriggersItem, wiredAllUserTriggerable, wiredDelay);
                }
            }
        }

        if (roomId == 0)
        {
            foreach (var item in this._floorItems.Values)
            {
                if (WiredUtillity.TypeIsWired(item.ItemData.InteractionType))
                {
                    item.WiredHandler?.LoadItems(true);
                }
            }
        }
    }

    public ICollection<Item> FloorItems => this._floorItems.Values;

    public ItemTemp GetFirstTempDrop(int x, int y)
    {
        foreach (var item in this._itemsTemp.Values)
        {
            if (item.InteractionType is not InteractionTypeTemp.RpItem and not InteractionTypeTemp.Money)
            {
                continue;
            }

            if (item.X != x || item.Y != y)
            {
                continue;
            }

            return item;
        }

        return null;
    }

    public ItemTemp GetTempItem(int pId)
    {
        if (this._itemsTemp != null && this._itemsTemp.ContainsKey(pId))
        {
            if (this._itemsTemp.TryGetValue(pId, out var item))
            {
                return item;
            }
        }

        return null;
    }

    public Item GetItem(int id)
    {
        if (this._floorItems != null && this._floorItems.ContainsKey(id))
        {
            if (this._floorItems.TryGetValue(id, out var item))
            {
                return item;
            }
        }
        else if (this._wallItems != null && this._wallItems.ContainsKey(id))
        {
            if (this._wallItems.TryGetValue(id, out var item))
            {
                return item;
            }
        }

        return null;
    }

    public ICollection<ItemTemp> TempItems => this._itemsTemp.Values;

    public ICollection<Item> WallItems => this._wallItems.Values;

    public IEnumerable<Item> WallAndFloorItems => this._floorItems.Values.Concat(this._wallItems.Values);

    public void RemoveFurniture(GameClient Session, int id)
    {
        var roomItem = this.GetItem(id);
        if (roomItem == null)
        {
            return;
        }

        roomItem.Interactor.OnRemove(Session, roomItem);
        this.RemoveRoomItem(roomItem);
        roomItem.Destroy();

        room.SendPacket(roomItem.IsWallItem ? new ItemRemoveComposer(roomItem.Id, room.RoomData.OwnerId) : new ObjectRemoveComposer(roomItem.Id, room.RoomData.OwnerId));
    }

    public void RemoveTempItem(int id)
    {
        var item = this.GetTempItem(id);
        if (item == null)
        {
            return;
        }

        room.SendPacket(new ObjectRemoveComposer(item.Id, 0));
        _ = this._itemsTemp.TryRemove(id, out _);
    }

    private void RemoveRoomItem(Item item)
    {
        if (item.IsWallItem)
        {
            _ = this._wallItems.TryRemove(item.Id, out _);
        }
        else
        {
            _ = this._floorItems.TryRemove(item.Id, out _);
            _ = room.GameMap.RemoveFromMap(item);
        }

        if (this._updateItems.ContainsKey(item.Id))
        {
            _ = this._updateItems.TryRemove(item.Id, out _);
        }

        if (this._rollers.ContainsKey(item.Id))
        {
            _ = this._rollers.TryRemove(item.Id, out _);
        }

        if (item.WiredHandler != null)
        {
            item.WiredHandler.Dispose();
            room.WiredHandler.RemoveFurniture(item);
            item.WiredHandler = null;
        }

        foreach (var threeDcoord in item.GetAffectedTiles)
        {
            var userForSquare = room.GameMap.GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y));
            if (userForSquare == null)
            {
                continue;
            }

            foreach (var user in userForSquare)
            {
                if (!user.IsWalking)
                {
                    room.RoomUserManager.UpdateUserStatus(user, false);
                }
            }
        }
    }

    private ServerPacketList CycleRollers()
    {
        if (this._rollerCycle >= this._rollerSpeed || this._rollerSpeed == 0)
        {
            this._rollerItemsMoved.Clear();
            this._rollerUsersMoved.Clear();
            this._rollerMessages.Clear();

            foreach (var roller in this._rollers.Values.ToList())
            {
                var nextCoord = roller.SquareInFront;
                var itemsOnRoller = room.GameMap.GetRoomItemForSquare(roller.X, roller.Y, roller.Z);
                var usersForSquare = room.RoomUserManager.GetUsersForSquare(roller.X, roller.Y);

                if (itemsOnRoller.Count > 0 || usersForSquare.Count > 0)
                {
                    if (itemsOnRoller.Count > 10)
                    {
                        itemsOnRoller = itemsOnRoller.Take(10).ToList();
                    }

                    var itemsOnNext = room.GameMap.GetCoordinatedItems(nextCoord);
                    var nextRoller = false;
                    var nextZ = 0.0;
                    var nextRollerClear = true;

                    foreach (var item in itemsOnNext)
                    {
                        if (item.ItemData.InteractionType == InteractionType.ROLLER)
                        {
                            nextRoller = true;
                            if (item.TotalHeight > nextZ)
                            {
                                nextZ = item.TotalHeight;
                            }
                        }
                    }
                    if (nextRoller)
                    {
                        foreach (var roomItem2 in itemsOnNext)
                        {
                            if (roomItem2.TotalHeight > nextZ)
                            {
                                nextRollerClear = false;
                            }
                        }
                    }
                    else
                    {
                        nextZ += room.GameMap.GetHeightForSquareFromData(nextCoord);
                    }

                    foreach (var item in itemsOnRoller)
                    {
                        var rollerHeight = item.Z - roller.TotalHeight;
                        if (!this._rollerItemsMoved.Contains(item.Id) && room.GameMap.CanStackItem(nextCoord.X, nextCoord.Y) && nextRollerClear && roller.Z < item.Z)
                        {
                            this._rollerMessages.Add(new SlideObjectBundleComposer(item.X, item.Y, item.Z, nextCoord.X, nextCoord.Y, nextZ + rollerHeight, item.Id));
                            this._rollerItemsMoved.Add(item.Id);

                            _ = this.SetFloorItem(item, nextCoord.X, nextCoord.Y, nextZ + rollerHeight);
                        }
                    }

                    foreach (var userForSquare in usersForSquare)
                    {
                        if (userForSquare != null && !userForSquare.SetStep && (userForSquare.AllowMoveToRoller || this._rollerSpeed == 0) &&
                            (!userForSquare.IsWalking || userForSquare.Freeze) && nextRollerClear && room.GameMap.CanWalk(nextCoord.X, nextCoord.Y) &&
                            room.GameMap.SquareTakingOpen(nextCoord.X, nextCoord.Y) && !this._rollerUsersMoved.Contains(userForSquare.UserId))
                        {
                            var userNextZ = nextZ;
                            if (userForSquare.RidingHorse && !userForSquare.IsPet)
                            {
                                var horseRoomUser = room.RoomUserManager.GetRoomUserByVirtualId(userForSquare.HorseID);
                                if (horseRoomUser != null)
                                {
                                    this._rollerMessages.Add(new SlideObjectBundleComposer(horseRoomUser.X, horseRoomUser.Y, horseRoomUser.Z, nextCoord.X, nextCoord.Y, nextZ, horseRoomUser.VirtualId, roller.Id, false));
                                    this._rollerUsersMoved.Add(horseRoomUser.UserId);

                                    horseRoomUser.SetPosRoller(nextCoord.X, nextCoord.Y, nextZ);
                                }
                                userNextZ += 1;
                            }
                            this._rollerMessages.Add(new SlideObjectBundleComposer(userForSquare.X, userForSquare.Y, userForSquare.Z, nextCoord.X, nextCoord.Y, userNextZ, userForSquare.VirtualId, roller.Id, false));
                            this._rollerUsersMoved.Add(userForSquare.UserId);

                            userForSquare.SetPosRoller(nextCoord.X, nextCoord.Y, nextZ);
                        }
                    }
                }
            }

            this._rollerCycle = 0;
            return this._rollerMessages;
        }
        else
        {
            this._rollerCycle++;
        }

        return new ServerPacketList();
    }

    public void PositionReset(Item item, int x, int y, double z, bool disableAnimation = false)
    {
        room.SendPacket(new SlideObjectBundleComposer(disableAnimation ? x : item.X, disableAnimation ? y : item.Y, disableAnimation ? z : item.Z, x, y, z, item.Id));

        _ = this.SetFloorItem(item, x, y, z);
    }

    public static ServerPacket TeleportUser(RoomUser user, Point nextCoord, int rollerID, double nextZ, bool disableAnimation = false)
    {
        var x = disableAnimation ? nextCoord.X : user.X;
        var y = disableAnimation ? nextCoord.Y : user.Y;
        var z = disableAnimation ? nextZ : user.Z;

        user.SetPos(nextCoord.X, nextCoord.Y, nextZ);

        return new SlideObjectBundleComposer(x, y, z, nextCoord.X, nextCoord.Y, nextZ, user.VirtualId, rollerID, false);
    }

    public void SaveFurniture(IDbConnection dbClient)
    {
        try
        {
            if (this._updateItems.IsEmpty && room.RoomUserManager.BotPetCount <= 0)
            {
                return;
            }

            if (!this._updateItems.IsEmpty)
            {
                ItemDao.SaveUpdateItems(dbClient, this._updateItems);

                this._updateItems.Clear();
            }

            room.RoomUserManager.SavePets(dbClient);
            room.RoomUserManager.SaveBots(dbClient);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogCriticalException("Error during saving furniture for room " + room.Id + ". Stack: " + ex.ToString());
        }
    }

    public ItemTemp AddTempItem(int itemId, int spriteId, int x, int y, double z, string extraData, int value = 0, InteractionTypeTemp pInteraction = InteractionTypeTemp.None, MovementDirection movement = MovementDirection.none, int pDistance = 0, int pTeamId = 0)
    {
        var id = this._itemTempoId--;
        var item = new ItemTemp(id, itemId, spriteId, x, y, z, extraData, movement, value, pInteraction, pDistance, pTeamId);

        if (!this._itemsTemp.ContainsKey(item.Id))
        {
            _ = this._itemsTemp.TryAdd(item.Id, item);
        }

        room.SendPacket(new ObjectAddComposer(item));

        return item;
    }

    public bool SetFloorItem(GameClient Session, Item item, int newX, int newY, int newRot, bool newItem, bool onRoller, bool sendMessage)
    {
        var needsReAdd = false;
        if (!newItem)
        {
            needsReAdd = room.GameMap.RemoveFromMap(item);
        }

        var affectedTiles = GameMap.GetAffectedTiles(item.ItemData.Length, item.ItemData.Width, newX, newY, newRot);
        foreach (var coord in affectedTiles)
        {
            if (!room.GameMap.ValidTile(coord.X, coord.Y) || (room.GameMap.SquareHasUsers(coord.X, coord.Y) && !item.ItemData.Walkable && !item.ItemData.IsSeat && item.ItemData.InteractionType != InteractionType.BED) || room.GameMap.Model.SqState[coord.X, coord.Y] != SquareStateType.Open)
            {
                if (needsReAdd)
                {
                    this.UpdateItem(item);
                    _ = room.GameMap.AddItemToMap(item);
                }
                return false;
            }
        }

        double pZ = room.GameMap.Model.SqFloorHeight[newX, newY];

        var itemsAffected = new List<Item>();
        var itemsComplete = new List<Item>();

        foreach (var threeDcoord in affectedTiles)
        {
            var temp = room.GameMap.GetCoordinatedItems(new Point(threeDcoord.X, threeDcoord.Y));
            if (temp != null)
            {
                itemsAffected.AddRange(temp);
            }
        }
        itemsComplete.AddRange(itemsAffected);

        var buildToolEnable = false;
        var buildToolStackHeight = false;
        var buildToolHeight = 1.0;
        var pileMagic = false;

        if (item.ItemData.InteractionType == InteractionType.PILE_MAGIC)
        {
            pileMagic = true;
        }

        if (Session != null && Session.User != null && Session.User.Room != null)
        {
            var roomUser = Session.User.Room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
            if (roomUser != null)
            {
                buildToolEnable = roomUser.BuildToolEnable;
                buildToolStackHeight = roomUser.BuildToolStackHeight;
                buildToolHeight = roomUser.BuildToolHeight;
            }
        }

        if (item.Rotation != newRot && item.X == newX && item.Y == newY && !buildToolStackHeight)
        {
            pZ = item.Z;
        }

        if (buildToolStackHeight)
        {
            pZ += buildToolHeight;
        }
        else
        {
            foreach (var roomItem in itemsComplete)
            {
                if (roomItem.ItemData.InteractionType == InteractionType.PILE_MAGIC)
                {
                    pZ = roomItem.Z;
                    pileMagic = true;
                    break;
                }
                if (roomItem.Id != item.Id && roomItem.TotalHeight > pZ)
                {
                    if (buildToolEnable)
                    {
                        pZ = roomItem.Z + buildToolHeight;
                    }
                    else
                    {
                        pZ = roomItem.TotalHeight;
                    }
                }
            }
        }

        if (!onRoller)
        {
            foreach (var roomItem in itemsComplete)
            {
                if (roomItem != null && roomItem.Id != item.Id && roomItem.ItemData != null && !roomItem.ItemData.Stackable && !buildToolEnable && !pileMagic && !buildToolStackHeight)
                {
                    if (needsReAdd)
                    {
                        this.UpdateItem(item);
                        _ = room.GameMap.AddItemToMap(item);
                    }
                    return false;
                }
            }
        }

        if (newRot is not 1 and not 2 and not 3 and not 4 and not 5 and not 6 and not 7 and not 8)
        {
            newRot = 0;
        }

        var userForSquare = new List<RoomUser>();

        foreach (var threeDcoord in item.GetAffectedTiles)
        {
            userForSquare.AddRange(room.GameMap.GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y)));
        }

        item.Rotation = newRot;
        item.SetState(newX, newY, pZ, true);

        if (!onRoller && Session != null)
        {
            item.Interactor.OnPlace(Session, item);
        }

        if (newItem)
        {
            if (this._floorItems.ContainsKey(item.Id))
            {
                Session?.SendNotification(LanguageManager.TryGetValue("room.itemplaced", Session.Language));

                return true;
            }
            else
            {
                if (item.IsFloorItem && !this._floorItems.ContainsKey(item.Id))
                {
                    _ = this._floorItems.TryAdd(item.Id, item);
                }
                else if (item.IsWallItem && !this._wallItems.ContainsKey(item.Id))
                {
                    _ = this._wallItems.TryAdd(item.Id, item);
                }

                this.UpdateItem(item);
                if (sendMessage)
                {
                    room.SendPacket(new ObjectAddComposer(item, room.RoomData.OwnerName, room.RoomData.OwnerId));
                }
            }
        }
        else
        {
            this.UpdateItem(item);
            if (!onRoller && sendMessage)
            {
                item.UpdateState(false);
            }
        }

        _ = room.GameMap.AddItemToMap(item);

        foreach (var threeDcoord in item.GetAffectedTiles)
        {
            userForSquare.AddRange(room.GameMap.GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y)));
        }

        foreach (var user in userForSquare)
        {
            if (user == null)
            {
                continue;
            }

            if (user.IsWalking)
            {
                continue;
            }

            room.RoomUserManager.UpdateUserStatus(user, false);
        }

        return true;
    }

    public void TryAddRoller(int itemId, Item roller) => this._rollers.TryAdd(itemId, roller);

    public ICollection<Item> Rollers => this._rollers.Values;

    public bool SetFloorItem(Item item, int newX, int newY, double newZ)
    {
        _ = room.GameMap.RemoveFromMap(item);
        item.SetState(newX, newY, newZ, true);
        this.UpdateItem(item);
        _ = room.GameMap.AddItemToMap(item);
        return true;
    }

    public bool SetWallItem(GameClient Session, Item item)
    {
        if (!item.IsWallItem || this._wallItems.ContainsKey(item.Id))
        {
            return false;
        }

        if (this._floorItems.ContainsKey(item.Id))
        {
            return true;
        }
        else
        {
            item.Interactor.OnPlace(Session, item);
            if (item.ItemData.InteractionType == InteractionType.MOODLIGHT && room.MoodlightData == null)
            {
                using var dbClient = DatabaseManager.Connection;

                var moodlightRow = ItemMoodlightDao.GetOne(dbClient, item.Id);

                var moodlightEnabled = moodlightRow != null && moodlightRow.Enabled;
                var moodlightCurrentPreset = moodlightRow != null ? moodlightRow.CurrentPreset : 1;
                var moodlightPresetOne = moodlightRow != null ? moodlightRow.PresetOne : "#000000,255,0";
                var moodlightPresetTwo = moodlightRow != null ? moodlightRow.PresetTwo : "#000000,255,0";
                var moodlightPresetThree = moodlightRow != null ? moodlightRow.PresetThree : "#000000,255,0";

                room.MoodlightData = new MoodlightData(item.Id, moodlightEnabled, moodlightCurrentPreset, moodlightPresetOne, moodlightPresetTwo, moodlightPresetThree);
                item.ExtraData = room.MoodlightData.GenerateExtraData();
            }
            _ = this._wallItems.TryAdd(item.Id, item);
            this.UpdateItem(item);

            room.SendPacket(new ItemAddComposer(item, room.RoomData.OwnerName, room.RoomData.OwnerId));

            return true;
        }
    }

    public void UpdateItem(Item item)
    {
        if (this._updateItems.ContainsKey(item.Id))
        {
            return;
        }

        _ = this._updateItems.TryAdd(item.Id, item);
    }

    public void OnCycle()
    {
        room.SendPackets(this.CycleRollers());

        if (!this._roomItemUpdateQueue.IsEmpty)
        {
            var addItems = new List<Item>();

            while (!this._roomItemUpdateQueue.IsEmpty)
            {
                if (this._roomItemUpdateQueue.TryDequeue(out var item))
                {
                    if (room.Disposed)
                    {
                        continue;
                    }

                    if (item.Room == null)
                    {
                        continue;
                    }

                    item.ProcessUpdates();

                    if (item.UpdateCounter > 0)
                    {
                        addItems.Add(item);
                    }
                }
            }
            foreach (var item in addItems)
            {
                this._roomItemUpdateQueue.Enqueue(item);
            }
        }
    }

    public void Destroy()
    {
        this._floorItems.Clear();
        this._wallItems.Clear();
        this._itemsTemp.Clear();
        this._updateItems.Clear();
        this._rollerUsersMoved.Clear();
        this._rollerMessages.Clear();
        this._rollerItemsMoved.Clear();
        this._roomItemUpdateQueue.Clear();
    }
}
