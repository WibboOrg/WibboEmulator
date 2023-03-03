namespace WibboEmulator.Games.Rooms;
using System.Collections.Concurrent;
using System.Data;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.Map.Movement;
using WibboEmulator.Games.Rooms.Moodlight;
using WibboEmulator.Utilities;

public class RoomItemHandling
{
    private readonly ConcurrentDictionary<int, Item> _floorItems;
    private readonly ConcurrentDictionary<int, Item> _wallItems;
    private readonly ConcurrentDictionary<int, Item> _rollers;

    private readonly ConcurrentDictionary<int, ItemTemp> _itemsTemp;

    private readonly ConcurrentDictionary<int, Item> _updateItems;

    private readonly List<int> _rollerItemsMoved;
    private readonly List<int> _rollerUsersMoved;
    private readonly ServerPacketList _rollerMessages;

    private int _rollerSpeed;
    private int _rollerCycle;
    private readonly ConcurrentQueue<Item> _roomItemUpdateQueue;
    private int _itemTempoId;

    private readonly Room _roomInstance;

    public RoomItemHandling(Room room)
    {
        this._roomInstance = room;
        this._updateItems = new ConcurrentDictionary<int, Item>();
        this._rollers = new ConcurrentDictionary<int, Item>();
        this._wallItems = new ConcurrentDictionary<int, Item>();
        this._floorItems = new ConcurrentDictionary<int, Item>();
        this._itemsTemp = new ConcurrentDictionary<int, ItemTemp>();
        this._itemTempoId = 0;
        this._roomItemUpdateQueue = new ConcurrentQueue<Item>();
        this._rollerCycle = 0;
        this._rollerSpeed = 4;
        this._rollerItemsMoved = new List<int>();
        this._rollerUsersMoved = new List<int>();
        this._rollerMessages = new ServerPacketList();
    }

    public void QueueRoomItemUpdate(Item item) => this._roomItemUpdateQueue.Enqueue(item);

    public List<Item> RemoveAllFurniture(GameClient session)
    {
        var listMessage = new ServerPacketList();
        var items = new List<Item>();

        foreach (var roomItem in this._floorItems.Values.ToList())
        {
            roomItem.Interactor.OnRemove(session, roomItem);

            roomItem.Destroy();
            listMessage.Add(new ObjectRemoveComposer(roomItem.Id, session.User.Id));
            items.Add(roomItem);
        }

        foreach (var roomItem in this._wallItems.Values.ToList())
        {
            roomItem.Interactor.OnRemove(session, roomItem);
            roomItem.Destroy();

            listMessage.Add(new ItemRemoveComposer(roomItem.Id, this._roomInstance.RoomData.OwnerId));
            items.Add(roomItem);
        }
        this._roomInstance.SendMessage(listMessage);

        this._wallItems.Clear();
        this._floorItems.Clear();
        this._itemsTemp.Clear();
        this._updateItems.Clear();
        this._rollers.Clear();
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.UpdateRoomIdAndUserId(dbClient, this._roomInstance.RoomData.OwnerId, this._roomInstance.Id);
        }

        this._roomInstance.GameMap.GenerateMaps();
        this._roomInstance.RoomUserManager.UpdateUserStatusses();
        this._roomInstance.WiredHandler.OnPickall();

        return items;
    }

    public void SetSpeed(int p) => this._rollerSpeed = p;

    public void LoadFurniture(int roomId = 0)
    {
        if (roomId == 0)
        {
            this._floorItems.Clear();
            this._wallItems.Clear();
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        int itemID;
        int userId;
        int baseID;
        string extraData;
        int x;
        int y;
        double z;
        sbyte rot;
        string wallposs;
        int limited;
        int limitedTo;
        string wallCoord;

        var itemTable = ItemDao.GetAll(dbClient, (roomId == 0) ? this._roomInstance.Id : roomId);

        foreach (DataRow dataRow in itemTable.Rows)
        {
            itemID = Convert.ToInt32(dataRow["id"]);
            userId = Convert.ToInt32(dataRow["user_id"]);
            baseID = Convert.ToInt32(dataRow["base_item"]);
            extraData = !DBNull.Value.Equals(dataRow["extra_data"]) ? (string)dataRow["extra_data"] : string.Empty;
            x = Convert.ToInt32(dataRow["x"]);
            y = Convert.ToInt32(dataRow["y"]);
            z = Convert.ToDouble(dataRow["z"]);
            rot = Convert.ToSByte(dataRow["rot"]);
            wallposs = !DBNull.Value.Equals(dataRow["wall_pos"]) ? (string)dataRow["wall_pos"] : string.Empty;
            limited = !DBNull.Value.Equals(dataRow["limited_number"]) ? Convert.ToInt32(dataRow["limited_number"]) : 0;
            limitedTo = !DBNull.Value.Equals(dataRow["limited_stack"]) ? Convert.ToInt32(dataRow["limited_stack"]) : 0;

            _ = WibboEnvironment.GetGame().GetItemManager().GetItem(baseID, out var data);

            if (data == null)
            {
                continue;
            }

            if (data.Type.ToString() == "i")
            {
                if (string.IsNullOrEmpty(wallposs))
                {
                    wallCoord = "w=0,0 l=0,0 l";
                }
                else
                {
                    wallCoord = wallposs;
                }

                var roomItem = new Item(itemID, this._roomInstance.Id, baseID, extraData, limited, limitedTo, 0, 0, 0.0, 0, wallCoord, this._roomInstance);
                if (!this._wallItems.ContainsKey(itemID))
                {
                    _ = this._wallItems.TryAdd(itemID, roomItem);
                }

                if (roomItem.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                {
                    this._roomInstance.MoodlightData ??= new MoodlightData(roomItem.Id);
                }
            }
            else //Is flooritem
            {
                var roomItem = new Item(itemID, this._roomInstance.Id, baseID, extraData, limited, limitedTo, x, y, z, rot, "", this._roomInstance);

                if (!this._floorItems.ContainsKey(itemID))
                {
                    _ = this._floorItems.TryAdd(itemID, roomItem);
                }
            }
        }

        if (roomId == 0)
        {
            foreach (var item in this._floorItems.Values)
            {
                if (WiredUtillity.TypeIsWired(item.GetBaseItem().InteractionType))
                {
                    WiredRegister.HandleRegister(item, this._roomInstance, dbClient);
                }
            }
        }
    }

    public ICollection<Item> GetFloor => this._floorItems.Values;

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

    public Item GetItem(int pId)
    {
        if (this._floorItems != null && this._floorItems.ContainsKey(pId))
        {
            if (this._floorItems.TryGetValue(pId, out var item))
            {
                return item;
            }
        }
        else if (this._wallItems != null && this._wallItems.ContainsKey(pId))
        {
            if (this._wallItems.TryGetValue(pId, out var item))
            {
                return item;
            }
        }

        return null;
    }

    public ICollection<ItemTemp> GetTempItems => this._itemsTemp.Values;

    public ICollection<Item> GetWall => this._wallItems.Values;

    public IEnumerable<Item> GetWallAndFloor => this._floorItems.Values.Concat(this._wallItems.Values);

    public void RemoveFurniture(GameClient session, int pId)
    {
        var roomItem = this.GetItem(pId);
        if (roomItem == null)
        {
            return;
        }

        roomItem.Interactor.OnRemove(session, roomItem);

        this.RemoveRoomItem(roomItem);

        roomItem.Destroy();
    }

    public void RemoveTempItem(int id)
    {
        var item = this.GetTempItem(id);
        if (item == null)
        {
            return;
        }

        this._roomInstance.SendPacket(new ObjectRemoveComposer(item.Id, 0));
        _ = this._itemsTemp.TryRemove(id, out _);
    }

    private void RemoveRoomItem(Item item)
    {
        if (item.IsWallItem)
        {
            this._roomInstance.SendPacket(new ItemRemoveComposer(item.Id, this._roomInstance.RoomData.OwnerId));
        }
        else if (item.IsFloorItem)
        {
            this._roomInstance.SendPacket(new ObjectRemoveComposer(item.Id, this._roomInstance.RoomData.OwnerId));
        }

        if (item.IsWallItem)
        {
            _ = this._wallItems.TryRemove(item.Id, out _);
        }
        else
        {
            _ = this._floorItems.TryRemove(item.Id, out _);
            _ = this._roomInstance.GameMap.RemoveFromMap(item);
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
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemWiredDao.Delete(dbClient, item.Id);
            }

            item.WiredHandler.Dispose();
            this._roomInstance.WiredHandler.RemoveFurniture(item);
            item.WiredHandler = null;
        }

        foreach (var threeDcoord in item.GetAffectedTiles)
        {
            var userForSquare = this._roomInstance.GameMap.GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y));
            if (userForSquare == null)
            {
                continue;
            }

            foreach (var user in userForSquare)
            {
                if (!user.IsWalking)
                {
                    this._roomInstance.RoomUserManager.UpdateUserStatus(user, false);
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
                var itemsOnRoller = this._roomInstance.GameMap.GetRoomItemForSquare(roller.X, roller.Y, roller.Z);
                var usersForSquare = this._roomInstance.RoomUserManager.GetUsersForSquare(roller.X, roller.Y);

                if (itemsOnRoller.Count > 0 || usersForSquare.Count > 0)
                {
                    if (itemsOnRoller.Count > 10)
                    {
                        itemsOnRoller = itemsOnRoller.Take(10).ToList();
                    }

                    var itemsOnNext = this._roomInstance.GameMap.GetCoordinatedItems(nextCoord);
                    var nextRoller = false;
                    var nextZ = 0.0;
                    var nextRollerClear = true;

                    foreach (var item in itemsOnNext)
                    {
                        if (item.GetBaseItem().InteractionType == InteractionType.ROLLER)
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
                        nextZ += this._roomInstance.GameMap.GetHeightForSquareFromData(nextCoord);
                    }

                    foreach (var item in itemsOnRoller)
                    {
                        var rollerHeight = item.Z - roller.TotalHeight;
                        if (!this._rollerItemsMoved.Contains(item.Id) && this._roomInstance.GameMap.CanStackItem(nextCoord.X, nextCoord.Y) && nextRollerClear && roller.Z < item.Z)
                        {
                            this._rollerMessages.Add(new SlideObjectBundleComposer(item.X, item.Y, item.Z, nextCoord.X, nextCoord.Y, nextZ + rollerHeight, item.Id));
                            this._rollerItemsMoved.Add(item.Id);

                            _ = this.SetFloorItem(item, nextCoord.X, nextCoord.Y, nextZ + rollerHeight);
                        }
                    }

                    foreach (var userForSquare in usersForSquare)
                    {
                        if (userForSquare != null && !userForSquare.SetStep && (userForSquare.AllowMoveToRoller || this._rollerSpeed == 0) &&
                            (!userForSquare.IsWalking || userForSquare.Freeze) && nextRollerClear && this._roomInstance.GameMap.CanWalk(nextCoord.X, nextCoord.Y) &&
                            this._roomInstance.GameMap.SquareTakingOpen(nextCoord.X, nextCoord.Y) && !this._rollerUsersMoved.Contains(userForSquare.UserId))
                        {
                            this._rollerMessages.Add(new SlideObjectBundleComposer(userForSquare.X, userForSquare.Y, userForSquare.Z, nextCoord.X, nextCoord.Y, nextZ, userForSquare.VirtualId, roller.Id, false));
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
        this._roomInstance.SendPacket(new SlideObjectBundleComposer(disableAnimation ? x : item.X, disableAnimation ? y : item.Y, disableAnimation ? z : item.Z, x, y, z, item.Id));

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

    public void SaveFurniture()
    {
        try
        {
            if (this._updateItems.IsEmpty && this._roomInstance.RoomUserManager.BotPetCount <= 0)
            {
                return;
            }

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

            if (!this._updateItems.IsEmpty)
            {
                ItemDao.SaveUpdateItems(dbClient, this._updateItems);

                this._updateItems.Clear();
            }

            this._roomInstance.RoomUserManager.SavePets(dbClient);
            this._roomInstance.RoomUserManager.SaveBots(dbClient);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogCriticalException("Error during saving furniture for room " + this._roomInstance.Id + ". Stack: " + ex.ToString());
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

        this._roomInstance.SendPacket(new ObjectAddComposer(item));

        return item;
    }

    public bool SetFloorItem(GameClient session, Item item, int newX, int newY, int newRot, bool newItem, bool onRoller, bool sendMessage)
    {
        var needsReAdd = false;
        if (!newItem)
        {
            needsReAdd = this._roomInstance.GameMap.RemoveFromMap(item);
        }

        var affectedTiles = GameMap.GetAffectedTiles(item.GetBaseItem().Length, item.GetBaseItem().Width, newX, newY, newRot);
        foreach (var coord in affectedTiles)
        {
            if (!this._roomInstance.GameMap.ValidTile(coord.X, coord.Y) || (this._roomInstance.GameMap.SquareHasUsers(coord.X, coord.Y) && !item.GetBaseItem().Walkable && !item.GetBaseItem().IsSeat && item.GetBaseItem().InteractionType != InteractionType.BED) || this._roomInstance.GameMap.Model.SqState[coord.X, coord.Y] != SquareStateType.Open)
            {
                if (needsReAdd)
                {
                    this.UpdateItem(item);
                    _ = this._roomInstance.GameMap.AddItemToMap(item);
                }
                return false;
            }
        }

        double pZ = this._roomInstance.GameMap.Model.SqFloorHeight[newX, newY];

        var itemsAffected = new List<Item>();
        var itemsComplete = new List<Item>();

        foreach (var threeDcoord in affectedTiles)
        {
            var temp = this._roomInstance.GameMap.GetCoordinatedItems(new Point(threeDcoord.X, threeDcoord.Y));
            if (temp != null)
            {
                itemsAffected.AddRange(temp);
            }
        }
        itemsComplete.AddRange(itemsAffected);

        var construitMode = false;
        var construitZMode = false;
        var construitHeigth = 1.0;
        var pileMagic = false;

        if (item.GetBaseItem().InteractionType == InteractionType.PILE_MAGIC)
        {
            pileMagic = true;
        }

        if (session != null && session.User != null && session.User.CurrentRoom != null)
        {
            var roomUser = session.User.CurrentRoom.RoomUserManager.GetRoomUserByUserId(session.User.Id);
            if (roomUser != null)
            {
                construitMode = roomUser.ConstruitEnable;
                construitZMode = roomUser.ConstruitZMode;
                construitHeigth = roomUser.ConstruitHeigth;
            }
        }

        if (item.Rotation != newRot && item.X == newX && item.Y == newY && !construitZMode)
        {
            pZ = item.Z;
        }

        if (construitZMode)
        {
            pZ += construitHeigth;
        }
        else
        {
            foreach (var roomItem in itemsComplete)
            {
                if (roomItem.GetBaseItem().InteractionType == InteractionType.PILE_MAGIC)
                {
                    pZ = roomItem.Z;
                    pileMagic = true;
                    break;
                }
                if (roomItem.Id != item.Id && roomItem.TotalHeight > pZ)
                {
                    if (construitMode)
                    {
                        pZ = roomItem.Z + construitHeigth;
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
                if (roomItem != null && roomItem.Id != item.Id && roomItem.GetBaseItem() != null && !roomItem.GetBaseItem().Stackable && !construitMode && !pileMagic && !construitZMode)
                {
                    if (needsReAdd)
                    {
                        this.UpdateItem(item);
                        _ = this._roomInstance.GameMap.AddItemToMap(item);
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
            userForSquare.AddRange(this._roomInstance.GameMap.GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y)));
        }

        item.Rotation = newRot;
        item.SetState(newX, newY, pZ, true);

        if (!onRoller && session != null)
        {
            item.Interactor.OnPlace(session, item);
        }

        if (newItem)
        {
            if (this._floorItems.ContainsKey(item.Id))
            {
                session?.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("room.itemplaced", session.Langue));

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
                    this._roomInstance.SendPacket(new ObjectAddComposer(item, this._roomInstance.RoomData.OwnerName, this._roomInstance.RoomData.OwnerId));
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

        _ = this._roomInstance.GameMap.AddItemToMap(item);

        foreach (var threeDcoord in item.GetAffectedTiles)
        {
            userForSquare.AddRange(this._roomInstance.GameMap.GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y)));
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

            this._roomInstance.RoomUserManager.UpdateUserStatus(user, false);
        }

        return true;
    }

    public void TryAddRoller(int itemId, Item roller) => this._rollers.TryAdd(itemId, roller);

    public ICollection<Item> GetRollers() => this._rollers.Values;

    public bool SetFloorItem(Item item, int newX, int newY, double newZ)
    {
        _ = this._roomInstance.GameMap.RemoveFromMap(item);
        item.SetState(newX, newY, newZ, true);
        this.UpdateItem(item);
        _ = this._roomInstance.GameMap.AddItemToMap(item);
        return true;
    }

    public bool SetWallItem(GameClient session, Item item)
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
            item.Interactor.OnPlace(session, item);
            if (item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT && this._roomInstance.MoodlightData == null)
            {
                this._roomInstance.MoodlightData = new MoodlightData(item.Id);
                item.ExtraData = this._roomInstance.MoodlightData.GenerateExtraData();
            }
            _ = this._wallItems.TryAdd(item.Id, item);
            this.UpdateItem(item);

            this._roomInstance.SendPacket(new ItemAddComposer(item, this._roomInstance.RoomData.OwnerName, this._roomInstance.RoomData.OwnerId));

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
        this._roomInstance.SendMessage(this.CycleRollers());

        if (!this._roomItemUpdateQueue.IsEmpty)
        {
            var addItems = new List<Item>();

            while (!this._roomItemUpdateQueue.IsEmpty)
            {
                if (this._roomItemUpdateQueue.TryDequeue(out var item))
                {
                    if (this._roomInstance.Disposed)
                    {
                        continue;
                    }

                    if (item.GetRoom() == null)
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
