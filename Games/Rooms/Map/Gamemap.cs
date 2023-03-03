namespace WibboEmulator.Games.Rooms.Map;
using System.Collections.Concurrent;
using System.Drawing;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map.Movement;
using WibboEmulator.Utilities;

public class GameMap
{
    private readonly Room _roomInstance;
    private readonly ConcurrentDictionary<Point, List<RoomUser>> _userMap;

    public bool DiagonalEnabled { get; set; }
    public bool ObliqueDisable { get; set; }

    public RoomModelDynamic Model { get; set; }
    public byte[,] EffectMap { get; private set; }
    public byte[,] MapGame { get; private set; }
    public double[,] ItemHeightMap { get; private set; }
    public byte[,] UserOnMap { get; private set; }
    public byte[,] SquareTaking { get; private set; }
    public ConcurrentDictionary<Point, List<Item>> CoordinatedItems { get; private set; }

    public GameMap(Room room)
    {
        this._roomInstance = room;
        this.ObliqueDisable = true;
        this.DiagonalEnabled = true;

        var staticModel = WibboEnvironment.GetGame().GetRoomManager().GetModel(room.RoomData.ModelName, room.Id) ?? throw new ArgumentNullException("No modeldata found for roomID " + room.Id);

        this.Model = new RoomModelDynamic(staticModel);

        this.CoordinatedItems = new ConcurrentDictionary<Point, List<Item>>();
        this._userMap = new ConcurrentDictionary<Point, List<RoomUser>>();

        this.MapGame = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.UserOnMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.SquareTaking = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.ItemHeightMap = new double[this.Model.MapSizeX, this.Model.MapSizeY];
        this.EffectMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
    }

    public void AddUserToMap(RoomUser user, Point coord)
    {
        if (this._userMap.TryGetValue(coord, out var value))
        {
            value.Add(user);
        }
        else
        {
            _ = this._userMap.TryAdd(coord, new List<RoomUser>() { user });
        }
        if (this.ValidTile(coord.X, coord.Y))
        {
            this.UserOnMap[coord.X, coord.Y] = 1;
        }
    }

    public static void TeleportToItem(RoomUser user, Item item)
    {
        if (item.GetRoom() != null)
        {
            user.RemoveStatus("mv");
            user.UpdateNeeded = true;
            item.GetRoom().SendPacket(RoomItemHandling.TeleportUser(user, item.Coordinate, 0, item.Z, true));
            item.GetRoom().RoomUserManager.UpdateUserStatus(user, false);
        }
    }

    public void UpdateUserMovement(Point oldCoord, Point newCoord, RoomUser user)
    {
        if (user.IsDispose)
        {
            return;
        }

        this.RemoveTakingSquare(user.SetX, user.SetY);
        this.RemoveUserFromMap(user, oldCoord);
        this.AddUserToMap(user, newCoord);
    }

    public void RemoveUserFromMap(RoomUser user, Point coord)
    {
        if (!this._userMap.ContainsKey(coord))
        {
            if (this.ValidTile(coord.X, coord.Y))
            {
                this.UserOnMap[coord.X, coord.Y] = 0;
            }

            return;
        }

        if (this._userMap[coord].Contains(user))
        {
            _ = this._userMap[coord].Remove(user);
        }

        if (this._userMap[coord].Count > 0)
        {
            return;
        }

        _ = this._userMap.TryRemove(coord, out _);

        if (this.ValidTile(coord.X, coord.Y))
        {
            this.UserOnMap[coord.X, coord.Y] = 0;
        }
    }

    public void AddTakingSquare(int x, int y)
    {
        if (this.ValidTile(x, y))
        {
            this.SquareTaking[x, y] = 1;
        }
    }

    public void RemoveTakingSquare(int x, int y)
    {
        if (this.ValidTile(x, y))
        {
            this.SquareTaking[x, y] = 0;
        }
    }

    public bool SquareTakingOpen(int x, int y)
    {
        if (!this.ValidTile(x, y))
        {
            return true;
        }

        return this._roomInstance.RoomData.AllowWalkthrough || this.SquareTaking[x, y] == 0;
    }

    public List<RoomUser> GetRoomUsers(Point coord)
    {
        if (this._userMap.TryGetValue(coord, out var value))
        {
            return value.ToList();
        }
        else
        {
            return new List<RoomUser>();
        }
    }

    public List<RoomUser> GetNearUsers(Point coord, int maxDistance)
    {
        var usersNear = new List<RoomUser>();

        foreach (var users in this._userMap)
        {
            if (Math.Abs(users.Key.X - coord.X) > maxDistance || Math.Abs(users.Key.Y - coord.Y) > maxDistance)
            {
                continue;
            }

            usersNear.AddRange(users.Value);
        }

        return usersNear.OrderBy(u => !u.IsBot).ToList();
    }

    public Point GetRandomWalkableSquare(int x, int y)
    {
        var rx = WibboEnvironment.GetRandomNumber(x - 5, x + 5);
        var ry = WibboEnvironment.GetRandomNumber(y - 5, y + 5);

        if (this.Model.DoorX == rx || this.Model.DoorY == ry || !this.CanWalk(rx, ry))
        {
            return new Point(x, y);
        }

        return new Point(rx, ry);
    }

    private void SetDefaultValue(int x, int y)
    {
        if (!this.ValidTile(x, y))
        {
            return;
        }

        this.MapGame[x, y] = 0;
        this.EffectMap[x, y] = 0;
        this.ItemHeightMap[x, y] = 0.0;

        if (this.Model.SqState[x, y] == SquareStateType.Open)
        {
            this.MapGame[x, y] = 1;
        }
    }

    public void UpdateMapForItem(Item item)
    {
        foreach (var coord in item.GetAffectedTiles)
        {
            if (!this.ConstructMapForItem(item, coord))
            {
                return;
            }
        }
    }

    public void GenerateMaps(bool checkLines = true)
    {
        var maxX = 0;
        var maxY = 0;
        if (checkLines)
        {
            var items = this._roomInstance.RoomItemHandling.GetFloor.ToArray();
            foreach (var item in items.ToList())
            {
                if (item == null)
                {
                    continue;
                }

                foreach (var point in item.GetAffectedTiles)
                {
                    if (point.X > maxX)
                    {
                        maxX = point.X;
                    }

                    if (point.Y > maxY)
                    {
                        maxY = point.Y;
                    }
                }
            }

            Array.Clear(items, 0, items.Length);
        }

        if (maxY > this.Model.MapSizeY - 1 || maxX > this.Model.MapSizeX - 1)
        {
            if (maxX < this.Model.MapSizeX)
            {
                maxX = this.Model.MapSizeX - 1;
            }

            if (maxY < this.Model.MapSizeY)
            {
                maxY = this.Model.MapSizeY - 1;
            }

            this.Model.SetMapsize(maxX + 1, maxY + 1);
        }

        this.CoordinatedItems.Clear();
        this.MapGame = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.UserOnMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.SquareTaking = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.ItemHeightMap = new double[this.Model.MapSizeX, this.Model.MapSizeY];
        this.EffectMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];

        for (var line = 0; line < this.Model.MapSizeY; ++line)
        {
            for (var chr = 0; chr < this.Model.MapSizeX; ++chr)
            {
                this.MapGame[chr, line] = 0;
                this.EffectMap[chr, line] = 0;

                if (this.Model.SqState[chr, line] == SquareStateType.Open)
                {
                    this.MapGame[chr, line] = 1;
                }
            }
        }

        foreach (var item in this._roomInstance.RoomItemHandling.GetFloor.ToArray())
        {
            if (!this.AddItemToMap(item))
            {
                continue;
            }
        }

        foreach (var user in this._roomInstance.RoomUserManager.GetUserList().ToList())
        {
            if (this.ValidTile(user.X, user.Y))
            {
                this.UserOnMap[user.X, user.Y] = 1;
            }
        }
    }

    private bool ConstructMapForItem(Item item, Point coord)
    {
        if (!this.ValidTile(coord.X, coord.Y))
        {
            return false;
        }
        else
        {
            if (this.ItemHeightMap[coord.X, coord.Y] <= item.TotalHeight)
            {
                if (item.GetBaseItem().IsSeat || item.GetBaseItem().InteractionType == InteractionType.BED)
                {
                    this.ItemHeightMap[coord.X, coord.Y] = item.Z - this.Model.SqFloorHeight[item.X, item.Y];
                }
                else
                {
                    this.ItemHeightMap[coord.X, coord.Y] = item.TotalHeight - this.Model.SqFloorHeight[item.X, item.Y];
                }

                this.EffectMap[coord.X, coord.Y] = item.GetBaseItem().InteractionType switch
                {
                    InteractionType.POOL => 1,
                    InteractionType.ICESKATES => 3,
                    InteractionType.NORMSLASKATES => 2,
                    InteractionType.LOWPOOL => 4,
                    InteractionType.HALLOWEENPOOL => 5,
                    InteractionType.TRAMPOLINE => 7,
                    InteractionType.TREADMILL => 8,
                    InteractionType.CROSSTRAINER => 9,
                    _ => 0,
                };
                if (item.GetBaseItem().InteractionType == InteractionType.FREEZE_TILE_BLOCK && item.ExtraData != "")
                {
                    if (this.MapGame[coord.X, coord.Y] != 3)
                    {
                        this.MapGame[coord.X, coord.Y] = 1;
                    }
                }
                else if (item.GetBaseItem().InteractionType == InteractionType.BANZAI_PYRAMID && item.ExtraData == "1")
                {
                    if (this.MapGame[coord.X, coord.Y] != 3)
                    {
                        this.MapGame[coord.X, coord.Y] = 1;
                    }
                }
                else if (item.GetBaseItem().InteractionType == InteractionType.GATE && item.ExtraData == "1")
                {
                    if (this.MapGame[coord.X, coord.Y] != 3)
                    {
                        this.MapGame[coord.X, coord.Y] = 1;
                    }
                }
                else if (item.GetBaseItem().Walkable)
                {
                    if (this.MapGame[coord.X, coord.Y] != 3)
                    {
                        this.MapGame[coord.X, coord.Y] = 1;
                    }
                }
                else if (!item.GetBaseItem().Walkable && item.GetBaseItem().Stackable)
                {
                    if (this.MapGame[coord.X, coord.Y] != 3)
                    {
                        this.MapGame[coord.X, coord.Y] = 2;
                    }
                }
                else if (item.GetBaseItem().IsSeat || item.GetBaseItem().InteractionType == InteractionType.BED)
                {
                    this.MapGame[coord.X, coord.Y] = 3;
                }
                else if (this.MapGame[coord.X, coord.Y] != 3)
                {
                    this.MapGame[coord.X, coord.Y] = 0;
                }
            }
        }
        return true;
    }

    public void AddCoordinatedItem(Item item, Point coord)
    {
        if (!this.CoordinatedItems.ContainsKey(coord))
        {
            _ = this.CoordinatedItems.TryAdd(coord, new List<Item>() { item });
        }
        else
        {
            var items = this.CoordinatedItems[coord];
            if (items.Contains(item))
            {
                return;
            }

            items.Add(item);
            this.CoordinatedItems[coord] = items;
        }
    }

    public List<Item> GetCoordinatedItems(Point coord)
    {
        var point = new Point(coord.X, coord.Y);
        if (this.CoordinatedItems.TryGetValue(point, out var value))
        {
            return value;
        }
        else
        {
            return new List<Item>();
        }
    }

    public bool RemoveCoordinatedItem(Item item, Point coord)
    {
        var point = new Point(coord.X, coord.Y);
        if (!this.CoordinatedItems.ContainsKey(point) || !this.CoordinatedItems[point].Contains(item))
        {
            return false;
        }
        _ = this.CoordinatedItems[point].Remove(item);
        return true;
    }

    private void AddSpecialItems(Item item)
    {
        switch (item.GetBaseItem().InteractionType)
        {
            case InteractionType.GUILD_GATE:
                this._roomInstance.GameItemHandler.AddGroupGate(item);
                break;
            case InteractionType.BANZAI_FLOOR:
                this._roomInstance.BattleBanzai.AddTile(item, item.Id);
                break;
            case InteractionType.BANZAI_TELE:
                this._roomInstance.GameItemHandler.AddTeleport(item, item.Id);
                item.ExtraData = "";
                break;
            case InteractionType.BANZAI_PYRAMID:
                this._roomInstance.GameItemHandler.AddPyramid(item, item.Id);
                break;
            case InteractionType.BANZAI_BLOB_2:
            case InteractionType.BANZAI_BLOB:
                this._roomInstance.GameItemHandler.AddBlob(item, item.Id);
                break;
            case InteractionType.FREEZE_EXIT:
                this._roomInstance.GameItemHandler.AddExitTeleport(item);
                break;
            case InteractionType.FREEZE_TILE_BLOCK:
                this._roomInstance.Freeze.AddFreezeBlock(item);
                break;
        }
    }

    private void RemoveSpecialItem(Item item)
    {
        switch (item.GetBaseItem().InteractionType)
        {
            case InteractionType.GUILD_GATE:
                this._roomInstance.GameItemHandler.RemoveGroupGate(item);
                break;
            case InteractionType.BANZAI_FLOOR:
                this._roomInstance.BattleBanzai.RemoveTile(item.Id);
                break;
            case InteractionType.BANZAI_TELE:
                this._roomInstance.GameItemHandler.RemoveTeleport(item.Id);
                break;
            case InteractionType.BANZAI_PYRAMID:
                this._roomInstance.GameItemHandler.RemovePyramid(item.Id);
                break;
            case InteractionType.BANZAI_BLOB_2:
            case InteractionType.BANZAI_BLOB:
                this._roomInstance.GameItemHandler.RemoveBlob(item.Id);
                break;
            case InteractionType.FREEZE_TILE_BLOCK:
                this._roomInstance.Freeze.RemoveFreezeBlock(item.Id);
                break;
            case InteractionType.FOOTBALL_GOAL_GREEN:
            case InteractionType.FOOTBALL_COUNTER_GREEN:
            case InteractionType.BANZAI_SCORE_GREEN:
            case InteractionType.BANZAI_GATE_GREEN:
            case InteractionType.FREEZE_GREEN_COUNTER:
            case InteractionType.FREEZE_GREEN_GATE:
                this._roomInstance.GameManager.RemoveFurnitureFromTeam(item, TeamType.Green);
                break;
            case InteractionType.FOOTBALL_GOAL_YELLOW:
            case InteractionType.FOOTBALL_COUNTER_YELLOW:
            case InteractionType.BANZAI_SCORE_YELLOW:
            case InteractionType.BANZAI_GATE_YELLOW:
            case InteractionType.FREEZE_YELLOW_COUNTER:
            case InteractionType.FREEZE_YELLOW_GATE:
                this._roomInstance.GameManager.RemoveFurnitureFromTeam(item, TeamType.Yellow);
                break;
            case InteractionType.FOOTBALL_GOAL_BLUE:
            case InteractionType.FOOTBALL_COUNTER_BLUE:
            case InteractionType.BANZAI_SCORE_BLUE:
            case InteractionType.BANZAI_GATE_BLUE:
            case InteractionType.FREEZE_BLUE_COUNTER:
            case InteractionType.FREEZE_BLUE_GATE:
                this._roomInstance.GameManager.RemoveFurnitureFromTeam(item, TeamType.Blue);
                break;
            case InteractionType.FOOTBALL_GOAL_RED:
            case InteractionType.FOOTBALL_COUNTER_RED:
            case InteractionType.BANZAI_SCORE_RED:
            case InteractionType.BANZAI_GATE_RED:
            case InteractionType.FREEZE_RED_COUNTER:
            case InteractionType.FREEZE_RED_GATE:
                this._roomInstance.GameManager.RemoveFurnitureFromTeam(item, TeamType.Red);
                break;
            case InteractionType.FREEZE_EXIT:
                this._roomInstance.GameItemHandler.RemoveExitTeleport(item);
                break;
        }
    }

    public bool RemoveFromMap(Item item)
    {
        if (WiredUtillity.TypeIsWired(item.GetBaseItem().InteractionType))
        {
            this._roomInstance.WiredHandler.RemoveFurniture(item);
        }

        this.RemoveSpecialItem(item);

        var flag = false;
        foreach (var coord in item.GetAffectedTiles)
        {
            if (this.RemoveCoordinatedItem(item, coord))
            {
                flag = true;
            }
        }

        var noDoublons = new Dictionary<Point, List<Item>>();
        foreach (var tile in item.GetAffectedTiles)
        {
            if (this.CoordinatedItems.TryGetValue(tile, out var value))
            {
                var list = value;
                if (!noDoublons.ContainsKey(tile))
                {
                    noDoublons.Add(tile, list);
                }
            }
            this.SetDefaultValue(tile.X, tile.Y);
        }

        foreach (var coord in noDoublons.Keys.ToList())
        {
            if (!noDoublons.ContainsKey(coord))
            {
                continue;
            }

            var subItems = noDoublons[coord];
            foreach (var roomItem in subItems.ToList())
            {
                _ = this.ConstructMapForItem(roomItem, coord);
            }
        }
        noDoublons.Clear();
        return flag;
    }

    public bool AddItemToMap(Item item)
    {
        if (WiredUtillity.TypeIsWired(item.GetBaseItem().InteractionType))
        {
            this._roomInstance.WiredHandler.AddFurniture(item);
        }

        this.AddSpecialItems(item);

        switch (item.GetBaseItem().InteractionType)
        {
            case InteractionType.ROLLER:
                if (!this._roomInstance.RoomItemHandling.GetRollers().Contains(item))
                {
                    this._roomInstance.RoomItemHandling.TryAddRoller(item.Id, item);
                }

                break;
            case InteractionType.FOOTBALL_GOAL_GREEN:
            case InteractionType.FOOTBALL_COUNTER_GREEN:
            case InteractionType.BANZAI_SCORE_GREEN:
            case InteractionType.BANZAI_GATE_GREEN:
            case InteractionType.FREEZE_GREEN_COUNTER:
            case InteractionType.FREEZE_GREEN_GATE:
                this._roomInstance.GameManager.AddFurnitureToTeam(item, TeamType.Green);
                break;
            case InteractionType.FOOTBALL_GOAL_YELLOW:
            case InteractionType.FOOTBALL_COUNTER_YELLOW:
            case InteractionType.BANZAI_SCORE_YELLOW:
            case InteractionType.BANZAI_GATE_YELLOW:
            case InteractionType.FREEZE_YELLOW_COUNTER:
            case InteractionType.FREEZE_YELLOW_GATE:
                this._roomInstance.GameManager.AddFurnitureToTeam(item, TeamType.Yellow);
                break;
            case InteractionType.FOOTBALL_GOAL_BLUE:
            case InteractionType.FOOTBALL_COUNTER_BLUE:
            case InteractionType.BANZAI_SCORE_BLUE:
            case InteractionType.BANZAI_GATE_BLUE:
            case InteractionType.FREEZE_BLUE_COUNTER:
            case InteractionType.FREEZE_BLUE_GATE:
                this._roomInstance.GameManager.AddFurnitureToTeam(item, TeamType.Blue);
                break;
            case InteractionType.FOOTBALL_GOAL_RED:
            case InteractionType.FOOTBALL_COUNTER_RED:
            case InteractionType.BANZAI_SCORE_RED:
            case InteractionType.BANZAI_GATE_RED:
            case InteractionType.FREEZE_RED_COUNTER:
            case InteractionType.FREEZE_RED_GATE:
                this._roomInstance.GameManager.AddFurnitureToTeam(item, TeamType.Red);
                break;
        }

        if (item.GetBaseItem().Type != 's')
        {
            return true;
        }

        foreach (var point in item.GetAffectedTiles)
        {
            this.AddCoordinatedItem(item, point);
        }

        if (item.GetBaseItem().InteractionType == InteractionType.FOOTBALL)
        {
            return true;
        }

        foreach (var coord in item.GetAffectedTiles)
        {
            if (!this.ConstructMapForItem(item, coord))
            {
                return false;
            }
        }
        return true;
    }

    public bool CanStackItem(int x, int y, bool noUser = false)
    {
        if (!this.ValidTile(x, y))
        {
            return false;
        }
        else
        {
            return (this.UserOnMap[x, y] == 0 || noUser) && (this.MapGame[x, y] == 1 || this.MapGame[x, y] == 2);
        }
    }

    public bool CanWalk(int x, int y, bool isOverride = false)
    {
        if (!this.ValidTile(x, y))
        {
            return false;
        }
        else
        {
            return (this._roomInstance.RoomData.AllowWalkthrough || isOverride || this.UserOnMap[x, y] == 0) && CanWalkState(this.MapGame[x, y], isOverride);
        };
    }

    public bool CanWalkState(int x, int y, bool isOverride)
    {
        if (!this.ValidTile(x, y))
        {
            return false;
        }
        else
        {
            return CanWalkState(this.MapGame[x, y], isOverride);
        }
    }

    public double GetHeightForSquareFromData(Point coord)
    {
        if (coord.X > this.Model.SqFloorHeight.GetUpperBound(0) || coord.Y > this.Model.SqFloorHeight.GetUpperBound(1) || coord.X < 0 || coord.Y < 0)
        {
            return 1.0;
        }
        else
        {
            return this.Model.SqFloorHeight[coord.X, coord.Y];
        }
    }

    public static bool CanWalkState(byte state, bool isOverride) => isOverride || state == 3 || state == 1;

    public bool ValidTile(int x, int y, double z = 0.0)
    {
        if (x < 0 || y < 0 || x >= this.Model.MapSizeX || y >= this.Model.MapSizeY || z > 1000)
        {
            return false;
        }

        return true;
    }

    public double SqAbsoluteHeight(int x, int y)
    {
        var point = new Point(x, y);
        if (!this.CoordinatedItems.ContainsKey(point))
        {
            return (double)this.GetHeightForSquareFromData(point);
        }

        var itemsOnSquare = this.CoordinatedItems[point];
        return this.SqAbsoluteHeight(x, y, itemsOnSquare);
    }

    public double SqAbsoluteHeight(int x, int y, List<Item> itemsOnSquare)
    {
        if (!this.ValidTile(x, y))
        {
            return 0.0;
        }

        var highestStack = 0.0;
        var deduct = false;
        var deductable = 0.0;
        foreach (var roomItem in itemsOnSquare)
        {
            if (roomItem.TotalHeight > highestStack)
            {
                if (roomItem.GetBaseItem().IsSeat || roomItem.GetBaseItem().InteractionType == InteractionType.BED)
                {
                    deduct = true;
                    deductable = roomItem.Height;
                }
                else
                {
                    deduct = false;
                }

                highestStack = roomItem.TotalHeight;
            }
        }
        double floorHeight = this.Model.SqFloorHeight[x, y];
        var stackHeight = highestStack - this.Model.SqFloorHeight[x, y];
        if (deduct)
        {
            stackHeight -= deductable;
        }

        if (stackHeight < 0.0)
        {
            stackHeight = 0.0;
        }

        return floorHeight + stackHeight;
    }

    public static List<Point> GetAffectedTiles(int length, int width, int posX, int posY, int rotation)
    {
        var pointList = new List<Point>
        {
            { new Point(posX, posY) }
        };

        if (length > 1)
        {
            if (rotation is 0 or 4)
            {
                for (var i = 1; i < length; i++)
                {
                    pointList.AddIfNotExists(new Point(posX, posY + i));

                    for (var j = 1; j < width; j++)
                    {
                        pointList.AddIfNotExists(new Point(posX + j, posY + i));
                    }
                }
            }
            else if (rotation is 2 or 6)
            {
                for (var i = 1; i < length; i++)
                {
                    pointList.AddIfNotExists(new Point(posX + i, posY));

                    for (var j = 1; j < width; j++)
                    {
                        pointList.AddIfNotExists(new Point(posX + i, posY + j));
                    }
                }
            }
        }

        if (width > 1)
        {
            if (rotation is 0 or 4)
            {
                for (var i = 1; i < width; i++)
                {
                    pointList.AddIfNotExists(new Point(posX + i, posY));

                    for (var j = 1; j < length; j++)
                    {
                        pointList.AddIfNotExists(new Point(posX + i, posY + j));
                    }
                }
            }
            else if (rotation is 2 or 6)
            {
                for (var i = 1; i < width; i++)
                {
                    pointList.AddIfNotExists(new Point(posX, posY + i));

                    for (var j = 1; j < length; j++)
                    {
                        pointList.AddIfNotExists(new Point(posX + j, posY + i));
                    }
                }
            }
        }

        return pointList.OrderBy(x => x.X + x.Y).ToList();
    }

    public List<Item> GetRoomItemForSquare(int pX, int pY, double minZ)
    {
        var list = new List<Item>();
        var point = new Point(pX, pY);
        if (this.CoordinatedItems.TryGetValue(point, out var value))
        {
            foreach (var roomItem in value)
            {
                if (roomItem.Z > minZ && roomItem.X == pX && roomItem.Y == pY)
                {
                    list.Add(roomItem);
                }
            }
        }
        return list;
    }

    public MovementState GetChasingMovement(int x, int y, MovementState oldMouvement)
    {
        var moveToLeft = true;
        var moveToRight = true;
        var moveToUp = true;
        var moveToDown = true;

        for (var i = 1; i < 4; i++)
        {
            // Left
            if (i == 1 && !this.CanStackItem(x - i, y))
            {
                moveToLeft = false;
            }
            else if (moveToLeft && this.SquareHasUsers(x - i, y))
            {
                return MovementState.left;
            }

            // Right
            if (i == 1 && !this.CanStackItem(x + i, y))
            {
                moveToRight = false;
            }
            else if (moveToRight && this.SquareHasUsers(x + i, y))
            {
                return MovementState.right;
            }

            // Up
            if (i == 1 && !this.CanStackItem(x, y - i))
            {
                moveToUp = false;
            }
            else if (moveToUp && this.SquareHasUsers(x, y - i))
            {
                return MovementState.up;
            }

            // Down
            if (i == 1 && !this.CanStackItem(x, y + i))
            {
                moveToDown = false;
            }
            else if (moveToDown && this.SquareHasUsers(x, y + i))
            {
                return MovementState.down;
            }

            // Breaking bucle
            if (i == 1 && !moveToLeft && !moveToRight && !moveToUp && !moveToDown)
            {
                return MovementState.none;
            }
        }

        var movements = new List<MovementState>();
        if (moveToLeft && oldMouvement != MovementState.right)
        {
            movements.Add(MovementState.left);
        }

        if (moveToRight && oldMouvement != MovementState.left)
        {
            movements.Add(MovementState.right);
        }

        if (moveToUp && oldMouvement != MovementState.down)
        {
            movements.Add(MovementState.up);
        }

        if (moveToDown && oldMouvement != MovementState.up)
        {
            movements.Add(MovementState.down);
        }

        if (movements.Count > 0)
        {
            return movements[WibboEnvironment.GetRandomNumber(0, movements.Count - 1)];
        }
        else
        {
            if (moveToLeft && oldMouvement == MovementState.left)
            {
                return MovementState.left;
            }

            if (moveToRight && oldMouvement == MovementState.right)
            {
                return MovementState.right;
            }

            if (moveToUp && oldMouvement == MovementState.up)
            {
                return MovementState.up;
            }

            if (moveToDown && oldMouvement == MovementState.down)
            {
                return MovementState.down;
            }
        }

        var movements2 = new List<MovementState>();
        if (moveToLeft)
        {
            movements2.Add(MovementState.left);
        }

        if (moveToRight)
        {
            movements2.Add(MovementState.right);
        }

        if (moveToUp)
        {
            movements2.Add(MovementState.up);
        }

        if (moveToDown)
        {
            movements2.Add(MovementState.down);
        }

        if (movements2.Count > 0)
        {
            return movements2[WibboEnvironment.GetRandomNumber(0, movements2.Count - 1)];
        }

        return MovementState.none;
    }

    public MovementState GetEscapeMovement(int x, int y, MovementState oldMouvement)
    {
        var moveToLeft = true;
        var moveToRight = true;
        var moveToUp = true;
        var moveToDown = true;

        for (var i = 1; i < 4; i++)
        {
            // Left
            if (i == 1 && !this.CanStackItem(x - i, y))
            {
                moveToLeft = false;
            }
            else if (moveToLeft && this.SquareHasUsers(x - i, y))
            {
                moveToLeft = false;
            }

            // Right
            if (i == 1 && !this.CanStackItem(x + i, y))
            {
                moveToRight = false;
            }
            else if (moveToRight && this.SquareHasUsers(x + i, y))
            {
                moveToRight = false;
            }

            // Up
            if (i == 1 && !this.CanStackItem(x, y - i))
            {
                moveToUp = false;
            }
            else if (moveToUp && this.SquareHasUsers(x, y - i))
            {
                moveToUp = false;
            }

            // Down
            if (i == 1 && !this.CanStackItem(x, y + i))
            {
                moveToDown = false;
            }
            else if (moveToDown && this.SquareHasUsers(x, y + i))
            {
                moveToDown = false;
            }

            // Breaking bucle
            if (i == 1 && !moveToLeft && !moveToRight && !moveToUp && !moveToDown)
            {
                return MovementState.none;
            }
        }

        var movements = new List<MovementState>();
        if (moveToLeft && oldMouvement != MovementState.right)
        {
            movements.Add(MovementState.left);
        }

        if (moveToRight && oldMouvement != MovementState.left)
        {
            movements.Add(MovementState.right);
        }

        if (moveToUp && oldMouvement != MovementState.down)
        {
            movements.Add(MovementState.up);
        }

        if (moveToDown && oldMouvement != MovementState.up)
        {
            movements.Add(MovementState.down);
        }

        if (movements.Count > 0)
        {
            return movements[WibboEnvironment.GetRandomNumber(0, movements.Count - 1)];
        }
        else
        {
            if (moveToLeft && oldMouvement == MovementState.left)
            {
                return MovementState.left;
            }

            if (moveToRight && oldMouvement == MovementState.right)
            {
                return MovementState.right;
            }

            if (moveToUp && oldMouvement == MovementState.up)
            {
                return MovementState.up;
            }

            if (moveToDown && oldMouvement == MovementState.down)
            {
                return MovementState.down;
            }
        }

        var movements2 = new List<MovementState>();
        if (moveToLeft)
        {
            movements2.Add(MovementState.left);
        }

        if (moveToRight)
        {
            movements2.Add(MovementState.right);
        }

        if (moveToUp)
        {
            movements2.Add(MovementState.up);
        }

        if (moveToDown)
        {
            movements2.Add(MovementState.down);
        }

        if (movements2.Count > 0)
        {
            return movements2[WibboEnvironment.GetRandomNumber(0, movements2.Count - 1)];
        }

        return MovementState.none;
    }

    public RoomUser SquareHasUserNear(int x, int y)
    {
        if (this.SquareHasUsers(x - 1, y))
        {
            return this._roomInstance.RoomUserManager.GetUserForSquare(x - 1, y);
        }
        else if (this.SquareHasUsers(x + 1, y))
        {
            return this._roomInstance.RoomUserManager.GetUserForSquare(x + 1, y);
        }
        else if (this.SquareHasUsers(x, y - 1))
        {
            return this._roomInstance.RoomUserManager.GetUserForSquare(x, y - 1);
        }
        else if (this.SquareHasUsers(x, y + 1))
        {
            return this._roomInstance.RoomUserManager.GetUserForSquare(x, y + 1);
        }

        return null;
    }

    public RoomUser LookHasUserNearNotBot(int x, int y, int distance = 0)
    {
        distance++;
        if (this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x - distance, y) != null)
        {
            return this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x - distance, y);
        }
        else if (this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x + distance, y) != null)
        {
            return this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x + distance, y);
        }
        else if (this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x, y - distance) != null)
        {
            return this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x, y - distance);
        }
        else if (this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x, y + distance) != null)
        {
            return this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x, y + distance);
        }
        //diago
        else if (this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x + distance, y + distance) != null)
        {
            return this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x + distance, y + distance);
        }
        else if (this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x - distance, y - distance) != null)
        {
            return this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x - distance, y + distance);
        }
        else if (this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x - distance, y + distance) != null)
        {
            return this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x - distance, y + distance);
        }
        else if (this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x + distance, y - distance) != null)
        {
            return this._roomInstance.RoomUserManager.GetUserForSquareNotBot(x + distance, y + distance);
        }

        return null;
    }

    public bool SquareHasUsers(int x, int y)
    {
        if (!this.ValidTile(x, y))
        {
            return false;
        }

        if (this.UserOnMap[x, y] == 0)
        {
            return false;
        }

        return true;
    }

    public static bool TilesTouching(int x1, int y1, int x2, int y2) => (Math.Abs(x1 - x2) <= 1 && Math.Abs(y1 - y2) <= 1) || (x1 == x2 && y1 == y2);

    public static int TileDistance(int x1, int y1, int x2, int y2) => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

    public void Destroy()
    {
        this._userMap.Clear();
        this.Model.Destroy();
        this.CoordinatedItems.Clear();
        Array.Clear(this.MapGame, 0, this.MapGame.Length);
        Array.Clear(this.EffectMap, 0, this.EffectMap.Length);
        Array.Clear(this.ItemHeightMap, 0, this.ItemHeightMap.Length);
        Array.Clear(this.UserOnMap, 0, this.UserOnMap.Length);
        Array.Clear(this.SquareTaking, 0, this.SquareTaking.Length);
    }
}
