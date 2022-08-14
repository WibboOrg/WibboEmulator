using WibboEmulator.Game.Items;
using WibboEmulator.Game.Items.Wired;
using WibboEmulator.Game.Rooms.Games;
using WibboEmulator.Game.Rooms.Map.Movement;
using WibboEmulator.Game.Rooms.PathFinding;
using System.Collections.Concurrent;
using System.Drawing;

namespace WibboEmulator.Game.Rooms
{
    public class Gamemap
    {
        private readonly Room _roomInstance;
        private readonly ConcurrentDictionary<Point, List<RoomUser>> _userMap;
        public bool DiagonalEnabled { get; set; }
        public bool ObliqueDisable { get; set; }

        public RoomModelDynamic Model;

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

            RoomModel mStaticModel = WibboEnvironment.GetGame().GetRoomManager().GetModel(room.RoomData.ModelName, room.Id);
            if (mStaticModel == null)
            {
                throw new Exception("No modeldata found for roomID " + room.Id);
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
                this._userMap.TryAdd(coord, new List<RoomUser>() { user });
            }
            if (this.ValidTile(coord.X, coord.Y))
            {
                this.UserOnMap[coord.X, coord.Y] = 1;
            }
        }

        public void TeleportToItem(RoomUser user, Item item)
        {
            if (item.GetRoom() != null)
            {
                item.GetRoom().SendPacket(user.Room.GetRoomItemHandler().TeleportUser(user, item.Coordinate, 0, item.Z, true));
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
                this._userMap[coord].Remove(user);
            }

            if (this._userMap[coord].Count > 0)
            {
                return;
            }

            this._userMap.TryRemove(coord, out List<RoomUser> UserList);

            if (this.ValidTile(coord.X, coord.Y))
            {
                this.UserOnMap[coord.X, coord.Y] = 0;
            }
        }

        public void AddTakingSquare(int X, int Y)
        {
            if (this.ValidTile(X, Y))
            {
                this.SquareTaking[X, Y] = 1;
            }
        }

        public void RemoveTakingSquare(int X, int Y)
        {
            if (this.ValidTile(X, Y))
            {
                this.SquareTaking[X, Y] = 0;
            }
        }

        public bool SquareTakingOpen(int X, int Y)
        {
            if (!this.ValidTile(X, Y))
            {
                return true;
            }

            return this._roomInstance.RoomData.AllowWalkthrough || this.SquareTaking[X, Y] == 0;
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

        public List<RoomUser> GetNearUsers(Point coord, int MaxDistance)
        {
            List<RoomUser> UsersNear = new List<RoomUser>();

            foreach (KeyValuePair<Point, List<RoomUser>> Users in this._userMap)
            {
                if (Math.Abs(Users.Key.X - coord.X) > MaxDistance || Math.Abs(Users.Key.Y - coord.Y) > MaxDistance)
                {
                    continue;
                }

                UsersNear.AddRange(Users.Value);
            }

            return UsersNear.OrderBy(u => !u.IsBot).ToList();
        }

        public Point getRandomWalkableSquare(int x, int y)
        {
            int rx = WibboEnvironment.GetRandomNumber(x - 5, x + 5);
            int ry = WibboEnvironment.GetRandomNumber(y - 5, y + 5);

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
            foreach (Point Coord in item.GetCoords)
            {
                if (!this.ConstructMapForItem(item, Coord))
                {
                    return;
                }
            }
        }

        public void GenerateMaps(bool checkLines = true)
        {
            int MaxX = 0;
            int MaxY = 0;
            if (checkLines)
            {
                Item[] items = this._roomInstance.GetRoomItemHandler().GetFloor.ToArray();
                foreach (Item item in items.ToList())
                {
                    if (item == null)
                    {
                        continue;
                    }

                    foreach (Coord Point in item.GetAffectedTiles.Values)
                    {
                        if (Point.X > MaxX)
                        {
                            MaxX = Point.X;
                        }

                        if (Point.Y > MaxY)
                        {
                            MaxY = Point.Y;
                        }
                    }
                }

                Array.Clear(items, 0, items.Length);
            }

            if (MaxY > (this.Model.MapSizeY - 1) || MaxX > (this.Model.MapSizeX - 1))
            {
                if (MaxX < this.Model.MapSizeX)
                {
                    MaxX = this.Model.MapSizeX - 1;
                }

                if (MaxY < this.Model.MapSizeY)
                {
                    MaxY = this.Model.MapSizeY - 1;
                }

                this.Model.SetMapsize(MaxX + 1, MaxY + 1);
            }

            this.CoordinatedItems.Clear();
            this.GameMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
            this.UserOnMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
            this.SquareTaking = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
            this.ItemHeightMap = new double[this.Model.MapSizeX, this.Model.MapSizeY];
            this.EffectMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];

            for (int line = 0; line < this.Model.MapSizeY; ++line)
            {
                for (int chr = 0; chr < this.Model.MapSizeX; ++chr)
                {
                    this.GameMap[chr, line] = 0;
                    this.EffectMap[chr, line] = 0;

                    if (this.Model.SqState[chr, line] == SquareStateType.OPEN)
                    {
                        this.GameMap[chr, line] = 1;
                    }
                }
            }

            foreach (Item Item in this._roomInstance.GetRoomItemHandler().GetFloor.ToArray())
            {
                if (!this.AddItemToMap(Item))
                {
                    continue;
                }
            }

            foreach (RoomUser user in this._roomInstance.GetRoomUserManager().GetUserList().ToList())
            {
                if (this.ValidTile(user.X, user.Y))
                {
                    this.UserOnMap[user.X, user.Y] = 1;
                }
            }
        }

        private bool ConstructMapForItem(Item Item, Point Coord)
        {
            if (!this.ValidTile(Coord.X, Coord.Y))
            {
                return false;
            }
            else
            {
                if (this.ItemHeightMap[Coord.X, Coord.Y] <= Item.TotalHeight)
                {
                    if (Item.GetBaseItem().IsSeat || Item.GetBaseItem().InteractionType == InteractionType.BED)
                    {
                        this.ItemHeightMap[Coord.X, Coord.Y] = Item.Z - this.Model.SqFloorHeight[Item.X, Item.Y];
                    }
                    else
                    {
                        this.ItemHeightMap[Coord.X, Coord.Y] = Item.TotalHeight - this.Model.SqFloorHeight[Item.X, Item.Y];
                    }

                    switch (Item.GetBaseItem().InteractionType)
                    {
                        case InteractionType.POOL:
                            this.EffectMap[Coord.X, Coord.Y] = 1;
                            break;
                        case InteractionType.ICESKATES:
                            this.EffectMap[Coord.X, Coord.Y] = 3;
                            break;
                        case InteractionType.NORMSLASKATES:
                            this.EffectMap[Coord.X, Coord.Y] = 2;
                            break;
                        case InteractionType.LOWPOOL:
                            this.EffectMap[Coord.X, Coord.Y] = 4;
                            break;
                        case InteractionType.HALOWEENPOOL:
                            this.EffectMap[Coord.X, Coord.Y] = 5;
                            break;
                        case InteractionType.TRAMPOLINE:
                            this.EffectMap[Coord.X, Coord.Y] = 7;
                            break;
                        case InteractionType.TREADMILL:
                            this.EffectMap[Coord.X, Coord.Y] = 8;
                            break;
                        case InteractionType.CROSSTRAINER:
                            this.EffectMap[Coord.X, Coord.Y] = 9;
                            break;
                        default:
                            this.EffectMap[Coord.X, Coord.Y] = 0;
                            break;
                    }

                    if (Item.GetBaseItem().InteractionType == InteractionType.FREEZETILEBLOCK && Item.ExtraData != "")
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                        {
                            this.GameMap[Coord.X, Coord.Y] = 1;
                        }
                    }
                    else if (Item.GetBaseItem().InteractionType == InteractionType.BANZAIPYRAMID && Item.ExtraData == "1")
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                        {
                            this.GameMap[Coord.X, Coord.Y] = 1;
                        }
                    }
                    else if (Item.GetBaseItem().InteractionType == InteractionType.GATE && Item.ExtraData == "1")
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                        {
                            this.GameMap[Coord.X, Coord.Y] = 1;
                        }
                    }
                    else if (Item.GetBaseItem().Walkable)
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                        {
                            this.GameMap[Coord.X, Coord.Y] = 1;
                        }
                    }
                    else if (!Item.GetBaseItem().Walkable && Item.GetBaseItem().Stackable)
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                        {
                            this.GameMap[Coord.X, Coord.Y] = 2;
                        }
                    }
                    else if (Item.GetBaseItem().IsSeat || Item.GetBaseItem().InteractionType == InteractionType.BED)
                    {
                        this.GameMap[Coord.X, Coord.Y] = 3;
                    }
                    else if (this.GameMap[Coord.X, Coord.Y] != 3)
                    {
                        this.GameMap[Coord.X, Coord.Y] = 0;
                    }
                }
            }
            return true;
        }

        public void AddCoordinatedItem(Item item, Point coord)
        {
            List<Item> list1 = new List<Item>();
            if (!this.CoordinatedItems.ContainsKey(coord))
            {
                this.CoordinatedItems.TryAdd(coord, new List<Item>() { item });
            }
            else
            {
                List<Item> list2 = this.CoordinatedItems[coord];
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
            Point point = new Point(coord.X, coord.Y);
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
            Point point = new Point(coord.X, coord.Y);
            if (!this.CoordinatedItems.ContainsKey(point) || !this.CoordinatedItems[point].Contains(item))
            {
                return false;
            }
            this.CoordinatedItems[point].Remove(item);
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

            bool flag = false;
            foreach (Point coord in item.GetCoords)
            {
                if (this.RemoveCoordinatedItem(item, coord))
                {
                    flag = true;
                }
            }

            Dictionary<Point, List<Item>> NoDoublons = new Dictionary<Point, List<Item>>();
            foreach (Point Tile in item.GetCoords.ToList())
            {
                Point point = new Point(Tile.X, Tile.Y);
                if (this.CoordinatedItems.ContainsKey(point))
                {
                    List<Item> list = this.CoordinatedItems[point];
                    if (!NoDoublons.ContainsKey(Tile))
                    {
                        NoDoublons.Add(Tile, list);
                    }
                }
                this.SetDefaultValue(Tile.X, Tile.Y);
            }

            foreach (Point Coord in NoDoublons.Keys.ToList())
            {
                if (!NoDoublons.ContainsKey(Coord))
                {
                    continue;
                }

                List<Item> SubItems = NoDoublons[Coord];
                foreach (Item roomItem in SubItems.ToList())
                {
                    this.ConstructMapForItem(roomItem, Coord);
                }
            }
            NoDoublons.Clear();
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

            foreach (Point point in item.GetCoords)
            {
                this.AddCoordinatedItem(item, new Point(point.X, point.Y));
            }

            if (item.GetBaseItem().InteractionType == InteractionType.FOOTBALL)
            {
                return true;
            }

            foreach (Point Coord in item.GetCoords)
            {
                if (!this.ConstructMapForItem(item, Coord))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanStackItem(int X, int Y, bool NoUser = false)
        {
            if (!this.ValidTile(X, Y))
            {
                return false;
            }
            else
            {
                return (this.UserOnMap[X, Y] == 0 || NoUser) && (this.GameMap[X, Y] == 1 || this.GameMap[X, Y] == 2);
            }
        }

        public bool CanWalk(int X, int Y, bool Override = false)
        {
            if (!this.ValidTile(X, Y))
            {
                return false;
            }
            else
            {
                return (this._roomInstance.RoomData.AllowWalkthrough || Override || this.UserOnMap[X, Y] == 0) && CanWalkState(this.GameMap[X, Y], Override);
            };
        }

        public bool CanWalkState(int X, int Y, bool Override)
        {
            if (!this.ValidTile(X, Y))
            {
                return false;
            }
            else
            {
                return CanWalkState(this.GameMap[X, Y], Override);
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

        public static bool CanWalkState(byte pState, bool pOverride)
        {
            return pOverride || pState == 3 || pState == 1;
        }

        public bool ValidTile(int X, int Y)
        {
            if (X < 0 || Y < 0 || X >= this.Model.MapSizeX || Y >= this.Model.MapSizeY)
            {
                return false;
            }

            return true;
        }

        public double SqAbsoluteHeight(int X, int Y)
        {
            Point point = new Point(X, Y);
            if (!this.CoordinatedItems.ContainsKey(point))
            {
                return (double)this.GetHeightForSquareFromData(point);
            }

            List<Item> ItemsOnSquare = this.CoordinatedItems[point];
            return this.SqAbsoluteHeight(X, Y, ItemsOnSquare);
        }

        public double SqAbsoluteHeight(int X, int Y, List<Item> ItemsOnSquare)
        {
            if (!this.ValidTile(X, Y))
            {
                return 0.0;
            }

            double HighestStack = 0.0;
            bool deduct = false;
            double deductable = 0.0;
            foreach (Item roomItem in ItemsOnSquare)
            {
                if (roomItem.TotalHeight > HighestStack)
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

                    HighestStack = roomItem.TotalHeight;
                }
            }
            double floorHeight = this.Model.SqFloorHeight[X, Y];
            double stackHeight = HighestStack - this.Model.SqFloorHeight[X, Y];
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

        public static Dictionary<int, Coord> GetAffectedTiles(int Length, int Width, int PosX, int PosY, int Rotation)
        {
            int num = 1;

            Dictionary<int, Coord> PointList = new Dictionary<int, Coord>
            {
                { 0, new Coord(PosX, PosY, 0) }
            };

            if (Length > 1)
            {
                if (Rotation == 0 || Rotation == 4)
                {
                    for (int z = 1; z < Length; z++)
                    {
                        PointList.Add(num++, new Coord(PosX, PosY + z, z));

                        for (int index = 1; index < Width; index++)
                        {
                            PointList.Add(num++, new Coord(PosX + index, PosY + z, (z < index) ? index : z));
                        }
                    }
                }
                else if (Rotation == 2 || Rotation == 6)
                {
                    for (int z = 1; z < Length; z++)
                    {
                        PointList.Add(num++, new Coord(PosX + z, PosY, z));
                        for (int index = 1; index < Width; index++)
                        {
                            PointList.Add(num++, new Coord(PosX + z, PosY + index, (z < index) ? index : z));
                        }
                    }
                }
            }

            if (Width > 1)
            {
                if (Rotation == 0 || Rotation == 4)
                {
                    for (int z = 1; z < Width; z++)
                    {
                        PointList.Add(num++, new Coord(PosX + z, PosY, z));
                        for (int index = 1; index < Length; index++)
                        {
                            PointList.Add(num++, new Coord(PosX + z, PosY + index, (z < index) ? index : z));
                        }
                    }
                }
                else if (Rotation == 2 || Rotation == 6)
                {
                    for (int z = 1; z < Width; z++)
                    {
                        PointList.Add(num++, new Coord(PosX, PosY + z, z));
                        for (int index = 1; index < Length; index++)
                        {
                            PointList.Add(num++, new Coord(PosX + index, PosY + z, (z < index) ? index : z));
                        }
                    }
                }
            }
            return PointList;
        }

        public List<Item> GetRoomItemForSquare(int pX, int pY, double minZ)
        {
            List<Item> list = new List<Item>();
            Point point = new Point(pX, pY);
            if (this.CoordinatedItems.ContainsKey(point))
            {
                foreach (Item roomItem in this.CoordinatedItems[point])
                {
                    if (roomItem.Z > minZ && roomItem.X == pX && roomItem.Y == pY)
                    {
                        list.Add(roomItem);
                    }
                }
            }
            return list;
        }

        public MovementState GetChasingMovement(int X, int Y, MovementState OldMouvement)
        {
            bool moveToLeft = true;
            bool moveToRight = true;
            bool moveToUp = true;
            bool moveToDown = true;

            for (int i = 1; i < 4; i++)
            {
                // Left
                if (i == 1 && !this.CanStackItem(X - i, Y))
                {
                    moveToLeft = false;
                }
                else if (moveToLeft && this.SquareHasUsers(X - i, Y))
                {
                    return MovementState.left;
                }

                // Right
                if (i == 1 && !this.CanStackItem(X + i, Y))
                {
                    moveToRight = false;
                }
                else if (moveToRight && this.SquareHasUsers(X + i, Y))
                {
                    return MovementState.right;
                }

                // Up
                if (i == 1 && !this.CanStackItem(X, Y - i))
                {
                    moveToUp = false;
                }
                else if (moveToUp && this.SquareHasUsers(X, Y - i))
                {
                    return MovementState.up;
                }

                // Down
                if (i == 1 && !this.CanStackItem(X, Y + i))
                {
                    moveToDown = false;
                }
                else if (moveToDown && this.SquareHasUsers(X, Y + i))
                {
                    return MovementState.down;
                }

                // Breaking bucle
                if (i == 1 && !moveToLeft && !moveToRight && !moveToUp && !moveToDown)
                {
                    return MovementState.none;
                }
            }

            List<MovementState> movements = new List<MovementState>();
            if (moveToLeft && OldMouvement != MovementState.right)
            {
                movements.Add(MovementState.left);
            }

            if (moveToRight && OldMouvement != MovementState.left)
            {
                movements.Add(MovementState.right);
            }

            if (moveToUp && OldMouvement != MovementState.down)
            {
                movements.Add(MovementState.up);
            }

            if (moveToDown && OldMouvement != MovementState.up)
            {
                movements.Add(MovementState.down);
            }

            if (movements.Count > 0)
            {
                return movements[new Random().Next(0, movements.Count)];
            }
            else
            {
                if (moveToLeft && OldMouvement == MovementState.left)
                {
                    return MovementState.left;
                }

                if (moveToRight && OldMouvement == MovementState.right)
                {
                    return MovementState.right;
                }

                if (moveToUp && OldMouvement == MovementState.up)
                {
                    return MovementState.up;
                }

                if (moveToDown && OldMouvement == MovementState.down)
                {
                    return MovementState.down;
                }
            }

            List<MovementState> movements2 = new List<MovementState>();
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
                return movements2[new Random().Next(0, movements2.Count)];
            }

            return MovementState.none;
        }

        public MovementState GetEscapeMovement(int X, int Y, MovementState OldMouvement)
        {
            bool moveToLeft = true;
            bool moveToRight = true;
            bool moveToUp = true;
            bool moveToDown = true;

            for (int i = 1; i < 4; i++)
            {
                // Left
                if (i == 1 && !this.CanStackItem(X - i, Y))
                {
                    moveToLeft = false;
                }
                else if (moveToLeft && this.SquareHasUsers(X - i, Y))
                {
                    moveToLeft = false;
                }

                // Right
                if (i == 1 && !this.CanStackItem(X + i, Y))
                {
                    moveToRight = false;
                }
                else if (moveToRight && this.SquareHasUsers(X + i, Y))
                {
                    moveToRight = false;
                }

                // Up
                if (i == 1 && !this.CanStackItem(X, Y - i))
                {
                    moveToUp = false;
                }
                else if (moveToUp && this.SquareHasUsers(X, Y - i))
                {
                    moveToUp = false;
                }

                // Down
                if (i == 1 && !this.CanStackItem(X, Y + i))
                {
                    moveToDown = false;
                }
                else if (moveToDown && this.SquareHasUsers(X, Y + i))
                {
                    moveToDown = false;
                }

                // Breaking bucle
                if (i == 1 && !moveToLeft && !moveToRight && !moveToUp && !moveToDown)
                {
                    return MovementState.none;
                }
            }

            List<MovementState> movements = new List<MovementState>();
            if (moveToLeft && OldMouvement != MovementState.right)
            {
                movements.Add(MovementState.left);
            }

            if (moveToRight && OldMouvement != MovementState.left)
            {
                movements.Add(MovementState.right);
            }

            if (moveToUp && OldMouvement != MovementState.down)
            {
                movements.Add(MovementState.up);
            }

            if (moveToDown && OldMouvement != MovementState.up)
            {
                movements.Add(MovementState.down);
            }

            if (movements.Count > 0)
            {
                return movements[new Random().Next(0, movements.Count)];
            }
            else
            {
                if (moveToLeft && OldMouvement == MovementState.left)
                {
                    return MovementState.left;
                }

                if (moveToRight && OldMouvement == MovementState.right)
                {
                    return MovementState.right;
                }

                if (moveToUp && OldMouvement == MovementState.up)
                {
                    return MovementState.up;
                }

                if (moveToDown && OldMouvement == MovementState.down)
                {
                    return MovementState.down;
                }
            }

            List<MovementState> movements2 = new List<MovementState>();
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
                return movements2[new Random().Next(0, movements2.Count)];
            }

            return MovementState.none;
        }

        public RoomUser SquareHasUserNear(int X, int Y, int Distance = 0)
        {
            if (this.SquareHasUsers(X - 1, Y))
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquare(X - 1, Y);
            }
            else if (this.SquareHasUsers(X + 1, Y))
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquare(X + 1, Y);
            }
            else if (this.SquareHasUsers(X, Y - 1))
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquare(X, Y - 1);
            }
            else if (this.SquareHasUsers(X, Y + 1))
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquare(X, Y + 1);
            }

            return null;
        }

        public RoomUser LookHasUserNearNotBot(int X, int Y, int Distance = 0)
        {
            Distance++;
            if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y) != null)
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y);
            }
            else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y) != null)
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y);
            }
            else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X, Y - Distance) != null)
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X, Y - Distance);
            }
            else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X, Y + Distance) != null)
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X, Y + Distance);
            }
            //diago
            else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y + Distance) != null)
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y + Distance);
            }
            else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y - Distance) != null)
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y + Distance);
            }
            else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y + Distance) != null)
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y + Distance);
            }
            else if (this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y - Distance) != null)
            {
                return this._roomInstance.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y + Distance);
            }

            return null;
        }

        public bool SquareHasUsers(int X, int Y)
        {
            if (!this.ValidTile(X, Y))
            {
                return false;
            }

            if (this.UserOnMap[X, Y] == 0)
            {
                return false;
            }

            return true;
        }

        public static bool TilesTouching(int X1, int Y1, int X2, int Y2)
        {
            return Math.Abs(X1 - X2) <= 1 && Math.Abs(Y1 - Y2) <= 1 || X1 == X2 && Y1 == Y2;
        }

        public static int TileDistance(int X1, int Y1, int X2, int Y2)
        {
            return Math.Abs(X1 - X2) + Math.Abs(Y1 - Y2);
        }

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
}
