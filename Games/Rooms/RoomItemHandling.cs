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

    private readonly Room _room;

    public RoomItemHandling(Room room)
    {
        this._room = room;
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
            listMessage.Add(new ObjectRemoveComposer(roomItem.Id, session.GetUser().Id));
            items.Add(roomItem);
        }

        foreach (var roomItem in this._wallItems.Values.ToList())
        {
            roomItem.Interactor.OnRemove(session, roomItem);
            roomItem.Destroy();

            listMessage.Add(new ItemRemoveComposer(roomItem.Id, this._room.RoomData.OwnerId));
            items.Add(roomItem);
        }
        this._room.SendMessage(listMessage);

        this._wallItems.Clear();
        this._floorItems.Clear();
        this._itemsTemp.Clear();
        this._updateItems.Clear();
        this._rollers.Clear();
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.UpdateRoomIdAndUserId(dbClient, this._room.RoomData.OwnerId, this._room.Id);
        }

        this._room.GetGameMap().GenerateMaps();
        this._room.GetRoomUserManager().UpdateUserStatusses();
        if (this._room.GotWired())
        {
            this._room.GetWiredHandler().OnPickall();
        }

        return items;
    }

    public void SetSpeed(int p) => this._rollerSpeed = p;

    public void LoadFurniture(int RoomId = 0)
    {
        if (RoomId == 0)
        {
            this._floorItems.Clear();
            this._wallItems.Clear();
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        int itemID;
        int UserId;
        int baseID;
        string ExtraData;
        int x;
        int y;
        double z;
        sbyte n;
        string wallposs;
        int Limited;
        int LimitedTo;
        string wallCoord;

        var itemTable = ItemDao.GetAll(dbClient, (RoomId == 0) ? this._room.Id : RoomId);

        foreach (DataRow dataRow in itemTable.Rows)
        {
            itemID = Convert.ToInt32(dataRow[0]);
            UserId = Convert.ToInt32(dataRow[1]);
            baseID = Convert.ToInt32(dataRow[3]);
            ExtraData = !DBNull.Value.Equals(dataRow[4]) ? (string)dataRow[4] : string.Empty;
            x = Convert.ToInt32(dataRow[5]);
            y = Convert.ToInt32(dataRow[6]);
            z = Convert.ToDouble(dataRow[7]);
            n = Convert.ToSByte(dataRow[8]);
            wallposs = !DBNull.Value.Equals(dataRow[9]) ? (string)dataRow[9] : string.Empty;
            Limited = !DBNull.Value.Equals(dataRow[10]) ? Convert.ToInt32(dataRow[10]) : 0;
            LimitedTo = !DBNull.Value.Equals(dataRow[11]) ? Convert.ToInt32(dataRow[11]) : 0;

            _ = WibboEnvironment.GetGame().GetItemManager().GetItem(baseID, out var Data);

            if (Data == null)
            {
                continue;
            }

            if (Data.Type.ToString() == "i")
            {
                if (string.IsNullOrEmpty(wallposs))
                {
                    wallCoord = "w=0,0 l=0,0 l";
                }
                else
                {
                    wallCoord = wallposs;
                }

                var roomItem = new Item(itemID, this._room.Id, baseID, ExtraData, Limited, LimitedTo, 0, 0, 0.0, 0, wallCoord, this._room);
                if (!this._wallItems.ContainsKey(itemID))
                {
                    _ = this._wallItems.TryAdd(itemID, roomItem);
                }

                if (roomItem.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                {
                    if (this._room.MoodlightData == null)
                    {
                        this._room.MoodlightData = new MoodlightData(roomItem.Id);
                    }
                }
            }
            else //Is flooritem
            {
                var roomItem = new Item(itemID, this._room.Id, baseID, ExtraData, Limited, LimitedTo, x, y, (double)z, n, "", this._room);

                if (!this._floorItems.ContainsKey(itemID))
                {
                    _ = this._floorItems.TryAdd(itemID, roomItem);
                }
            }
        }

        if (RoomId == 0)
        {
            foreach (var Item in this._floorItems.Values)
            {
                if (WiredUtillity.TypeIsWired(Item.GetBaseItem().InteractionType))
                {
                    WiredRegister.HandleRegister(Item, this._room, dbClient);
                }
            }
        }
    }

    public ICollection<Item> GetFloor => this._floorItems.Values;

    public ItemTemp GetFirstTempDrop(int x, int y)
    {
        foreach (var Item in this._itemsTemp.Values)
        {
            if (Item.InteractionType is not InteractionTypeTemp.RPITEM and not InteractionTypeTemp.MONEY)
            {
                continue;
            }

            if (Item.X != x || Item.Y != y)
            {
                continue;
            }

            return Item;
        }

        return null;
    }

    public ItemTemp GetTempItem(int pId)
    {
        if (this._itemsTemp != null && this._itemsTemp.ContainsKey(pId))
        {
            if (this._itemsTemp.TryGetValue(pId, out var Item))
            {
                return Item;
            }
        }

        return null;
    }

    public Item GetItem(int pId)
    {
        if (this._floorItems != null && this._floorItems.ContainsKey(pId))
        {
            if (this._floorItems.TryGetValue(pId, out var Item))
            {
                return Item;
            }
        }
        else if (this._wallItems != null && this._wallItems.ContainsKey(pId))
        {
            if (this._wallItems.TryGetValue(pId, out var Item))
            {
                return Item;
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

    public void RemoveTempItem(int pId)
    {
        var item = this.GetTempItem(pId);
        if (item == null)
        {
            return;
        }

        this._room.SendPacket(new ObjectRemoveComposer(item.Id, 0));
        _ = this._itemsTemp.TryRemove(pId, out item);
    }

    private void RemoveRoomItem(Item item)
    {
        if (item.IsWallItem)
        {
            this._room.SendPacket(new ItemRemoveComposer(item.Id, this._room.RoomData.OwnerId));
        }
        else if (item.IsFloorItem)
        {
            this._room.SendPacket(new ObjectRemoveComposer(item.Id, this._room.RoomData.OwnerId));
        }

        if (item.IsWallItem)
        {
            _ = this._wallItems.TryRemove(item.Id, out var itemRemoved);
        }
        else
        {
            _ = this._floorItems.TryRemove(item.Id, out var itemRemoved);
            _ = this._room.GetGameMap().RemoveFromMap(item);
        }

        if (this._updateItems.ContainsKey(item.Id))
        {
            _ = this._updateItems.TryRemove(item.Id, out var itemRemoved);
        }

        if (this._rollers.ContainsKey(item.Id))
        {
            _ = this._rollers.TryRemove(item.Id, out var itemRemoved);
        }

        if (item.WiredHandler != null)
        {
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemWiredDao.Delete(dbClient, item.Id);
            }

            item.WiredHandler.Dispose();
            this._room.GetWiredHandler().RemoveFurniture(item);
            item.WiredHandler = null;
        }

        foreach (var threeDcoord in item.GetAffectedTiles.Values)
        {
            var userForSquare = this._room.GetGameMap().GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y));
            if (userForSquare == null)
            {
                continue;
            }

            foreach (var User in userForSquare)
            {
                if (!User.IsWalking)
                {
                    this._room.GetRoomUserManager().UpdateUserStatus(User, false);
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
                var itemsOnRoller = this._room.GetGameMap().GetRoomItemForSquare(roller.X, roller.Y, roller.Z);
                var usersForSquare = this._room.GetRoomUserManager().GetUsersForSquare(roller.X, roller.Y);

                if (itemsOnRoller.Count > 0 || usersForSquare.Count > 0)
                {
                    if (itemsOnRoller.Count > 10)
                    {
                        itemsOnRoller = itemsOnRoller.Take(10).ToList();
                    }

                    var itemsOnNext = this._room.GetGameMap().GetCoordinatedItems(nextCoord);
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
                        nextZ += this._room.GetGameMap().GetHeightForSquareFromData(nextCoord);
                    }

                    foreach (var item in itemsOnRoller)
                    {
                        var rollerHeight = item.Z - roller.TotalHeight;
                        if (!this._rollerItemsMoved.Contains(item.Id) && this._room.GetGameMap().CanStackItem(nextCoord.X, nextCoord.Y) && nextRollerClear && roller.Z < item.Z)
                        {
                            this._rollerMessages.Add(new SlideObjectBundleComposer(item.X, item.Y, item.Z, nextCoord.X, nextCoord.Y, nextZ + rollerHeight, item.Id));
                            this._rollerItemsMoved.Add(item.Id);

                            _ = this.SetFloorItem(item, nextCoord.X, nextCoord.Y, nextZ + rollerHeight);
                        }
                    }

                    foreach (var userForSquare in usersForSquare)
                    {
                        if (userForSquare != null && !userForSquare.SetStep && (userForSquare.AllowMoveToRoller || this._rollerSpeed == 0) &&
                            (!userForSquare.IsWalking || userForSquare.Freeze) && nextRollerClear && this._room.GetGameMap().CanWalk(nextCoord.X, nextCoord.Y) &&
                            this._room.GetGameMap().SquareTakingOpen(nextCoord.X, nextCoord.Y) && !this._rollerUsersMoved.Contains(userForSquare.UserId))
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

    public void PositionReset(Item item, int x, int y, double z)
    {
        this._room.SendPacket(new SlideObjectBundleComposer(item.X, item.Y, item.Z, x, y, z, item.Id));

        _ = this.SetFloorItem(item, x, y, z);
    }

    public void RotReset(Item pItem, int newRot)
    {
        pItem.Rotation = newRot;

        this._room.SendPacket(new ObjectUpdateComposer(pItem, this._room.RoomData.OwnerId));
    }

    public ServerPacket TeleportUser(RoomUser user, Point nextCoord, int rollerID, double nextZ, bool noAnimation = false)
    {
        var x = noAnimation ? nextCoord.X : user.X;
        var y = noAnimation ? nextCoord.Y : user.Y;
        var z = noAnimation ? nextZ : user.Z;

        user.SetPos(nextCoord.X, nextCoord.Y, nextZ);

        return new SlideObjectBundleComposer(x, y, z, nextCoord.X, nextCoord.Y, nextZ, user.VirtualId, rollerID, false);
    }

    public void SaveFurniture()
    {
        try
        {
            if (this._updateItems.IsEmpty && this._room.GetRoomUserManager().BotPetCount <= 0)
            {
                return;
            }

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

            if (!this._updateItems.IsEmpty)
            {
                ItemDao.SaveUpdateItems(dbClient, this._updateItems);

                this._updateItems.Clear();
            }

            this._room.GetRoomUserManager().SavePets(dbClient);
            this._room.GetRoomUserManager().SaveBots(dbClient);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogCriticalException("Error during saving furniture for room " + this._room.Id + ". Stack: " + ex.ToString());
        }
    }

    public ItemTemp AddTempItem(int itemId, int spriteId, int x, int y, double z, string extraData, int value = 0, InteractionTypeTemp pInteraction = InteractionTypeTemp.NONE, MovementDirection movement = MovementDirection.none, int pDistance = 0, int pTeamId = 0)
    {
        var id = this._itemTempoId--;
        var Item = new ItemTemp(id, itemId, spriteId, x, y, z, extraData, movement, value, pInteraction, pDistance, pTeamId);

        if (!this._itemsTemp.ContainsKey(Item.Id))
        {
            _ = this._itemsTemp.TryAdd(Item.Id, Item);
        }

        this._room.SendPacket(new ObjectAddComposer(Item));

        return Item;
    }

    public bool SetFloorItem(GameClient session, Item Item, int newX, int newY, int newRot, bool newItem, bool OnRoller, bool sendMessage)
    {
        var NeedsReAdd = false;
        if (!newItem)
        {
            NeedsReAdd = this._room.GetGameMap().RemoveFromMap(Item);
        }

        var affectedTiles = Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, newX, newY, newRot);
        foreach (var threeDcoord in affectedTiles.Values)
        {
            if (!this._room.GetGameMap().ValidTile(threeDcoord.X, threeDcoord.Y) || (this._room.GetGameMap().SquareHasUsers(threeDcoord.X, threeDcoord.Y) && !Item.GetBaseItem().IsSeat && Item.GetBaseItem().InteractionType != InteractionType.BED) || this._room.GetGameMap().Model.SqState[threeDcoord.X, threeDcoord.Y] != SquareStateType.OPEN)
            {
                if (NeedsReAdd)
                {
                    this.UpdateItem(Item);
                    _ = this._room.GetGameMap().AddItemToMap(Item);
                }
                return false;
            }
        }

        double pZ = this._room.GetGameMap().Model.SqFloorHeight[newX, newY];

        var ItemsAffected = new List<Item>();
        var ItemsComplete = new List<Item>();

        foreach (var threeDcoord in affectedTiles.Values)
        {
            var Temp = this._room.GetGameMap().GetCoordinatedItems(new Point(threeDcoord.X, threeDcoord.Y));
            if (Temp != null)
            {
                ItemsAffected.AddRange(Temp);
            }
        }
        ItemsComplete.AddRange(ItemsAffected);

        var ConstruitMode = false;
        var ConstruitZMode = false;
        var ConstruitHeigth = 1.0;
        var PileMagic = false;

        if (Item.GetBaseItem().InteractionType == InteractionType.PILEMAGIC)
        {
            PileMagic = true;
        }

        if (session != null && session.GetUser() != null && session.GetUser().CurrentRoom != null)
        {
            var roomUser = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            if (roomUser != null)
            {
                ConstruitMode = roomUser.ConstruitEnable;
                ConstruitZMode = roomUser.ConstruitZMode;
                ConstruitHeigth = roomUser.ConstruitHeigth;
            }
        }

        if (Item.Rotation != newRot && Item.X == newX && Item.Y == newY && !ConstruitZMode)
        {
            pZ = Item.Z;
        }

        if (ConstruitZMode)
        {
            pZ += ConstruitHeigth;
        }
        else
        {
            foreach (var roomItem in ItemsComplete)
            {
                if (roomItem.GetBaseItem().InteractionType == InteractionType.PILEMAGIC)
                {
                    pZ = roomItem.Z;
                    PileMagic = true;
                    break;
                }
                if (roomItem.Id != Item.Id && roomItem.TotalHeight > pZ)
                {
                    if (ConstruitMode)
                    {
                        pZ = roomItem.Z + ConstruitHeigth;
                    }
                    else
                    {
                        pZ = roomItem.TotalHeight;
                    }
                }
            }
        }

        if (!OnRoller)
        {
            foreach (var roomItem in ItemsComplete)
            {
                if (roomItem != null && roomItem.Id != Item.Id && roomItem.GetBaseItem() != null && !roomItem.GetBaseItem().Stackable && !ConstruitMode && !PileMagic && !ConstruitZMode)
                {
                    if (NeedsReAdd)
                    {
                        this.UpdateItem(Item);
                        _ = this._room.GetGameMap().AddItemToMap(Item);
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

        foreach (var threeDcoord in Item.GetAffectedTiles.Values)
        {
            userForSquare.AddRange(this._room.GetGameMap().GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y)));
        }

        Item.Rotation = newRot;
        Item.SetState(newX, newY, pZ, affectedTiles);

        if (!OnRoller && session != null)
        {
            Item.Interactor.OnPlace(session, Item);
        }

        if (newItem)
        {
            if (this._floorItems.ContainsKey(Item.Id))
            {
                if (session != null)
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("room.itemplaced", session.Langue));
                }

                return true;
            }
            else
            {
                if (Item.IsFloorItem && !this._floorItems.ContainsKey(Item.Id))
                {
                    _ = this._floorItems.TryAdd(Item.Id, Item);
                }
                else if (Item.IsWallItem && !this._wallItems.ContainsKey(Item.Id))
                {
                    _ = this._wallItems.TryAdd(Item.Id, Item);
                }

                this.UpdateItem(Item);
                if (sendMessage)
                {
                    this._room.SendPacket(new ObjectAddComposer(Item, this._room.RoomData.OwnerName, this._room.RoomData.OwnerId));
                }
            }
        }
        else
        {
            this.UpdateItem(Item);
            if (!OnRoller && sendMessage)
            {
                this._room.SendPacket(new ObjectUpdateComposer(Item, this._room.RoomData.OwnerId));
            }
        }

        _ = this._room.GetGameMap().AddItemToMap(Item);

        foreach (var threeDcoord in Item.GetAffectedTiles.Values)
        {
            userForSquare.AddRange(this._room.GetGameMap().GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y)));
        }

        foreach (var User in userForSquare)
        {
            if (User == null)
            {
                continue;
            }

            if (User.IsWalking)
            {
                continue;
            }

            this._room.GetRoomUserManager().UpdateUserStatus(User, false);
        }

        return true;
    }

    public void TryAddRoller(int ItemId, Item Roller) => this._rollers.TryAdd(ItemId, Roller);

    public ICollection<Item> GetRollers() => this._rollers.Values;

    public bool SetFloorItem(Item Item, int newX, int newY, double newZ)
    {
        _ = this._room.GetGameMap().RemoveFromMap(Item);
        Item.SetState(newX, newY, newZ, Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, newX, newY, Item.Rotation));
        this.UpdateItem(Item);
        _ = this._room.GetGameMap().AddItemToMap(Item);
        return true;
    }

    public bool SetWallItem(GameClient session, Item Item)
    {
        if (!Item.IsWallItem || this._wallItems.ContainsKey(Item.Id))
        {
            return false;
        }

        if (this._floorItems.ContainsKey(Item.Id))
        {
            return true;
        }
        else
        {
            Item.Interactor.OnPlace(session, Item);
            if (Item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT && this._room.MoodlightData == null)
            {
                this._room.MoodlightData = new MoodlightData(Item.Id);
                Item.ExtraData = this._room.MoodlightData.GenerateExtraData();
            }
            _ = this._wallItems.TryAdd(Item.Id, Item);
            this.UpdateItem(Item);

            this._room.SendPacket(new ItemAddComposer(Item, this._room.RoomData.OwnerName, this._room.RoomData.OwnerId));

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
        this._room.SendMessage(this.CycleRollers());

        if (!this._roomItemUpdateQueue.IsEmpty)
        {
            var addItems = new List<Item>();

            while (!this._roomItemUpdateQueue.IsEmpty)
            {
                if (this._roomItemUpdateQueue.TryDequeue(out var item))
                {
                    if (this._room.Disposed)
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
    }
}
