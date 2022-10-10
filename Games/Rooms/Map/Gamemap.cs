namespace WibboEmulator.Games.Rooms.Map;
using System.Collections.Concurrent;
using System.Drawing;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map.Movement;

public class Gamemap
{
    private readonly Room _roomInstance;
    private readonly ConcurrentDictionary<Point, List<RoomUser>> _userMap;
    public bool DiagonalEnabled { get; set; }
    public bool ObliqueDisable { get; set; }

    public RoomModelDynamic Model { get; set; }

    public byte[,] EffectMap { get; private set; }

    public byte[,] GameMap { get; private set; }

    public double[,] ItemHeightMap { get; private set; }

    public byte[,] UserOnMap { get; private set; }

    public byte[,] SquareTaking { get; private set; }
    public ConcurrentDictionary<Point, List<Item>> CoordinatedItems { get; private set; }

    public Gamemap(Room room)
    {
        this._roomInstance = room;
        this.ObliqueDisable = true;
        this.DiagonalEnabled = true;

        var mStaticModel = WibboEnvironment.GetGame().GetRoomManager().GetModel(room.RoomData.ModelName, room.Id);
        if (mStaticModel == null)
        {
            throw new ArgumentNullException("No modeldata found for roomID " + room.Id);
        }

        this.Model = new RoomModelDynamic(mStaticModel);

        this.CoordinatedItems = new ConcurrentDictionary<Point, List<Item>>();
        this._userMap = new ConcurrentDictionary<Point, List<RoomUser>>();

        this.GameMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.UserOnMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.SquareTaking = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.ItemHeightMap = new double[this.Model.MapSizeX, this.Model.MapSizeY];
        this.EffectMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
    }

    public void AddUserToMap(RoomUser user, Point coord)
    {
        if (this._userMap.ContainsKey(coord))
        {
            this._userMap[coord].Add(user);
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
            item.GetRoom().SendPacket(RoomItemHandling.TeleportUser(user, item.Coordinate, 0, item.Z, true));
            item.GetRoom().GetRoomUserManager().UpdateUserStatus(user, false);
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
        if (this._userMap.ContainsKey(coord))
        {
            return this._userMap[coord];
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

        this.GameMap[x, y] = 0;
        this.EffectMap[x, y] = 0;
        this.ItemHeightMap[x, y] = 0.0;

        if (this.Model.SqState[x, y] == SquareStateType.OPEN)
        {
            this.GameMap[x, y] = 1;
        }
    }

    public void UpdateMapForItem(Item item)
    {
        foreach (var coord in item.GetAffectedTiles.Values)
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
            var items = this._roomInstance.GetRoomItemHandler().GetFloor.ToArray();
            foreach (var item in items.ToList())
            {
                if (item == null)
                {
                    continue;
                }

                foreach (var point in item.GetAffectedTiles.Values)
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
        this.GameMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.UserOnMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.SquareTaking = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        this.ItemHeightMap = new double[this.Model.MapSizeX, this.Model.MapSizeY];
        this.EffectMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];

        for (var line = 0; line < this.Model.MapSizeY; ++line)
        {
            for (var chr = 0; chr < this.Model.MapSizeX; ++chr)
            {
                this.GameMap[chr, line] = 0;
                this.EffectMap[chr, line] = 0;

                if (this.Model.SqState[chr, line] == SquareStateType.OPEN)
                {
                    this.GameMap[chr, line] = 1;
                }
            }
        }

        foreach (var item in this._roomInstance.GetRoomItemHandler().GetFloor.ToArray())
        {
            if (!this.AddItemToMap(item))
            {
                continue;
            }
        }

        foreach (var user in this._roomInstance.GetRoomUserManager().GetUserList().ToList())
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
                    InteractionType.HALOWEENPOOL => 5,
                    InteractionType.TRAMPOLINE => 7,
                    InteractionType.TREADMILL => 8,
                    InteractionType.CROSSTRAINER => 9,
                    _ => 0,
                };
                if (item.GetBaseItem().InteractionType == InteractionType.FREEZETILEBLOCK && item.ExtraData != "")
                {
                    if (this.GameMap[coord.X, coord.Y] != 3)
                    {
                        this.GameMap[coord.X, coord.Y] = 1;
                    }
                }
                else if (item.GetBaseItem().InteractionType == InteractionType.BANZAIPYRAMID && item.ExtraData == "1")
                {
                    if (this.GameMap[coord.X, coord.Y] != 3)
                    {
                        this.GameMap[coord.X, coord.Y] = 1;
                    }
                }
                else if (item.GetBaseItem().InteractionType == InteractionType.GATE && item.ExtraData == "1")
                {
                    if (this.GameMap[coord.X, coord.Y] != 3)
                    {
                        this.GameMap[coord.X, coord.Y] = 1;
                    }
                }
                else if (item.GetBaseItem().Walkable)
                {
                    if (this.GameMap[coord.X, coord.Y] != 3)
                    {
                        this.GameMap[coord.X, coord.Y] = 1;
                    }
                }
                else if (!item.GetBaseItem().Walkable && item.GetBaseItem().Stackable)
                {
                    if (this.GameMap[coord.X, coord.Y] != 3)
                    {
                        this.GameMap[coord.X, coord.Y] = 2;
                    }
                }
                else if (item.GetBaseItem().IsSeat || item.GetBaseItem().InteractionType == InteractionType.BED)
                {
                    this.GameMap[coord.X, coord.Y] = 3;
                }
                else if (this.GameMap[coord.X, coord.Y] != 3)
                {
                    this.GameMap[coord.X, coord.Y] = 0;
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
            var list2 = this.CoordinatedItems[coord];
            if (list2.Contains(item))
            {
                return;
            }

            list2.Add(item);
            this.CoordinatedItems[coord] = list2;
        }
    }

    public List<Item> GetCoordinatedItems(Point coord)
    {
        var point = new Point(coord.X, coord.Y);
        if (this.CoordinatedItems.ContainsKey(point))
        {
            return this.CoordinatedItems[point];
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
                this._roomInstance.GetGameItemHandler().AddGroupGate(item);
                break;
            case InteractionType.BANZAIFLOOR:
                this._roomInstance.GetBanzai().AddTile(item, item.Id);
                break;
            case InteractionType.BANZAITELE:
                this._roomInstance.GetGameItemHandler().AddTeleport(item, item.Id);
                item.ExtraData = "";
                break;
            case InteractionType.BANZAIPYRAMID:
                this._roomInstance.GetGameItemHandler().AddPyramid(item, item.Id);
                break;
            case InteractionType.BANZAIBLO:
            case InteractionType.BANZAIBLOB:
                this._roomInstance.GetGameItemHandler().AddBlob(item, item.Id);
                break;
            case InteractionType.FREEZEEXIT:
                this._roomInstance.GetGameItemHandler().AddExitTeleport(item);
                break;
            case InteractionType.FREEZETILEBLOCK:
                this._roomInstance.GetFreeze().AddFreezeBlock(item);
                break;
        }
    }

    private void RemoveSpecialItem(Item item)
    {
        switch (item.GetBaseItem().InteractionType)
        {
            case InteractionType.GUILD_GATE:
                this._roomInstance.GetGameItemHandler().RemoveGroupGate(item);
                break;
            case InteractionType.BANZAIFLOOR:
                this._roomInstance.GetBanzai().RemoveTile(item.Id);
                break;
            case InteractionType.BANZAITELE:
                this._roomInstance.GetGameItemHandler().RemoveTeleport(item.Id);
                break;
            case InteractionType.BANZAIPYRAMID:
                this._roomInstance.GetGameItemHandler().RemovePyramid(item.Id);
                break;
            case InteractionType.BANZAIBLO:
            case InteractionType.BANZAIBLOB:
                this._roomInstance.GetGameItemHandler().RemoveBlob(item.Id);
                break;
            case InteractionType.FREEZETILEBLOCK:
                this._roomInstance.GetFreeze().RemoveFreezeBlock(item.Id);
                break;
            case InteractionType.FOOTBALLGOALGREEN:
            case InteractionType.FOOTBALLCOUNTERGREEN:
            case InteractionType.BANZAISCOREGREEN:
            case InteractionType.BANZAIGATEGREEN:
            case InteractionType.FREEZEGREENCOUNTER:
            case InteractionType.FREEZEGREENGATE:
                this._roomInstance.GetGameManager().RemoveFurnitureFromTeam(item, TeamType.GREEN);
                break;
            case InteractionType.FOOTBALLGOALYELLOW:
            case InteractionType.FOOTBALLCOUNTERYELLOW:
            case InteractionType.BANZAISCOREYELLOW:
            case InteractionType.BANZAIGATEYELLOW:
            case InteractionType.FREEZEYELLOWCOUNTER:
            case InteractionType.FREEZEYELLOWGATE:
                this._roomInstance.GetGameManager().RemoveFurnitureFromTeam(item, TeamType.YELLOW);
                break;
            case InteractionType.footballgoalblue:
            case InteractionType.FOOTBALLCOUNTERBLUE:
            case InteractionType.BANZAISCOREBLUE:
            case InteractionType.BANZAIGATEBLUE:
            case InteractionType.FREEZEBLUECOUNTER:
            case InteractionType.FREEZEBLUEGATE:
                this._roomInstance.GetGameManager().RemoveFurnitureFromTeam(item, TeamType.BLUE);
                break;
            case InteractionType.FOOTBALLGOALRED:
            case InteractionType.FOOTBALLCOUNTERRED:
            case InteractionType.BANZAISCORERED:
            case InteractionType.BANZAIGATERED:
            case InteractionType.FREEZEREDCOUNTER:
            case InteractionType.FREEZEREDGATE:
                this._roomInstance.GetGameManager().RemoveFurnitureFromTeam(item, TeamType.RED);
                break;
            case InteractionType.FREEZEEXIT:
                this._roomInstance.GetGameItemHandler().RemoveExitTeleport(item);
                break;
        }
    }

    public bool RemoveFromMap(Item item)
    {
        if (this._roomInstance.GotWired() && WiredUtillity.TypeIsWired(item.GetBaseItem().InteractionType))
        {
            this._roomInstance.GetWiredHandler().RemoveFurniture(item);
        }

        this.RemoveSpecialItem(item);

        var flag = false;
        foreach (var coord in item.GetAffectedTiles.Values)
        {
            if (this.RemoveCoordinatedItem(item, coord))
            {
                flag = true;
            }
        }

        var noDoublons = new Dictionary<Point, List<Item>>();
        foreach (var tile in item.GetAffectedTiles.Values.ToList())
        {
            if (this.CoordinatedItems.ContainsKey(tile))
            {
                var list = this.CoordinatedItems[tile];
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
        if (this._roomInstance.GotWired() && WiredUtillity.TypeIsWired(item.GetBaseItem().InteractionType))
        {
            this._roomInstance.GetWiredHandler().AddFurniture(item);
        }

        this.AddSpecialItems(item);

        switch (item.GetBaseItem().InteractionType)
        {
            case InteractionType.ROLLER:
                if (!this._roomInstance.GetRoomItemHandler().GetRollers().Contains(item))
                {
                    this._roomInstance.GetRoomItemHandler().TryAddRoller(item.Id, item);
                }

                break;
            case InteractionType.FOOTBALLGOALGREEN:
            case InteractionType.FOOTBALLCOUNTERGREEN:
            case InteractionType.BANZAISCOREGREEN:
            case InteractionType.BANZAIGATEGREEN:
            case InteractionType.FREEZEGREENCOUNTER:
            case InteractionType.FREEZEGREENGATE:
                this._roomInstance.GetGameManager().AddFurnitureToTeam(item, TeamType.GREEN);
                break;
            case InteractionType.FOOTBALLGOALYELLOW:
            case InteractionType.FOOTBALLCOUNTERYELLOW:
            case InteractionType.BANZAISCOREYELLOW:
            case InteractionType.BANZAIGATEYELLOW:
            case InteractionType.FREEZEYELLOWCOUNTER:
            case InteractionType.FREEZEYELLOWGATE:
                this._roomInstance.GetGameManager().AddFurnitureToTeam(item, TeamType.YELLOW);
                break;
            case InteractionType.footballgoalblue:
            case InteractionType.FOOTBALLCOUNTERBLUE:
            case InteractionType.BANZAISCOREBLUE:
            case InteractionType.BANZAIGATEBLUE:
            case InteractionType.FREEZEBLUECOUNTER:
            case InteractionType.FREEZEBLUEGATE:
                this._roomInstance.GetGameManager().AddFurnitureToTeam(item, TeamType.BLUE);
                break;
            case InteractionType.FOOTBALLGOALRED:
            case InteractionType.FOOTBALLCOUNTERRED:
            case InteractionType.BANZAISCORERED:
            case InteractionType.BANZAIGATERED:
            case InteractionType.FREEZEREDCOUNTER:
            case InteractionType.FREEZEREDGATE:
                this._roomInstance.GetGameManager().AddFurnitureToTeam(item, TeamType.RED);
                break;
        }

        if (item.GetBaseItem().Type != 's')
        {
            return true;
        }

        foreach (var point in item.GetAffectedTiles.Values)
        {
            this.AddCoordinatedItem(item, point);
        }

        if (item.GetBaseItem().InteractionType == InteractionType.FOOTBALL)
        {
            return true;
        }

        foreach (var coord in item.GetAffectedTiles.Values)
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
            return (this.UserOnMap[x, y] == 0 || noUser) && (this.GameMap[x, y] == 1 || this.GameMap[x, y] == 2);
        }
    }

    public bool CanWalk(int x, int y, bool @override = false)
    {
        if (!this.ValidTile(x, y))
        {
            return false;
        }
        else
        {
            return (this._roomInstance.RoomData.AllowWalkthrough || @override || this.UserOnMap[x, y] == 0) && CanWalkState(this.GameMap[x, y], @override);
        };
    }

    public bool CanWalkState(int x, int y, bool @override)
    {
        if (!this.ValidTile(x, y))
        {
            return false;
        }
        else
        {
            return CanWalkState(this.GameMap[x, y], @override);
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

    public bool ValidTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= this.Model.MapSizeX || y >= this.Model.MapSizeY)
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

    public static Dictionary<int, Point> GetAffectedTiles(int length, int width, int posX, int posY, int rotation)
    {
        var num = 1;

        var pointList = new Dictionary<int, Point>
        {
            { 0, new Point(posX, posY) }
        };

        if (length > 1)
        {
            if (rotation is 0 or 4)
            {
                for (var z = 1; z < length; z++)
                {
                    pointList.Add(num++, new Point(posX, posY + z));

                    for (var index = 1; index < width; index++)
                    {
                        pointList.Add(num++, new Point(posX + index, posY + z));
                    }
                }
            }
            else if (rotation is 2 or 6)
            {
                for (var z = 1; z < length; z++)
                {
                    pointList.Add(num++, new Point(posX + z, posY));
                    for (var index = 1; index < width; index++)
                    {
                        pointList.Add(num++, new Point(posX + z, posY + index));
                    }
                }
            }
        }

        if (width > 1)
        {
            if (rotation is 0 or 4)
            {
                for (var z = 1; z < width; z++)
                {
                    pointList.Add(num++, new Point(posX + z, posY));
                    for (var index = 1; index < length; index++)
                    {
                        pointList.Add(num++, new Point(posX + z, posY + index));
                    }
                }
            }
            else if (rotation is 2 or 6)
            {
                for (var z = 1; z < width; z++)
                {
                    pointList.Add(num++, new Point(posX, posY + z));
                    for (var index = 1; index < length; index++)
                    {
                        pointList.Add(num++, new Point(posX + index, posY + z));
                    }
                }
            }
        }

        return pointList;
    }

    public List<Item> GetRoomItemForSquare(int pX, int pY, double minZ)
    {
        var list = new List<Item>();
        var point = new Point(pX, pY);
        if (this.CoordinatedItems.ContainsKey(point))
        {
            foreach (var roomItem in this.CoordinatedItems[point])
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
            return this._roomInstance.GetRoomUserManager().GetUserForSquare(x - 1, y);
        }
        else if (this.SquareHasUsers(x + 1, y))
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquare(x + 1, y);
        }
        else if (this.SquareHasUsers(x, y - 1))
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquare(x, y - 1);
        }
        else if (this.SquareHasUsers(x, y + 1))
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquare(x, y + 1);
        }

        return null;
    }

    public RoomUser LookHasUserNearNotBot(int x, int y, int distance = 0)
    {
        distance++;
        if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x - distance, y) != null)
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x - distance, y);
        }
        else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x + distance, y) != null)
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x + distance, y);
        }
        else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x, y - distance) != null)
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x, y - distance);
        }
        else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x, y + distance) != null)
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x, y + distance);
        }
        //diago
        else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x + distance, y + distance) != null)
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x + distance, y + distance);
        }
        else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x - distance, y - distance) != null)
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x - distance, y + distance);
        }
        else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x - distance, y + distance) != null)
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x - distance, y + distance);
        }
        else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x + distance, y - distance) != null)
        {
            return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(x + distance, y + distance);
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
        Array.Clear(this.GameMap, 0, this.GameMap.Length);
        Array.Clear(this.EffectMap, 0, this.EffectMap.Length);
        Array.Clear(this.ItemHeightMap, 0, this.ItemHeightMap.Length);
        Array.Clear(this.UserOnMap, 0, this.UserOnMap.Length);
        Array.Clear(this.SquareTaking, 0, this.SquareTaking.Length);
    }
}
