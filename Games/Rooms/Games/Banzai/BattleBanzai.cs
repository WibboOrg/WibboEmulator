namespace WibboEmulator.Games.Rooms.Games.Banzai;
using System.Collections;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.GameCenter;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Utilities.Enclosure;

public class BattleBanzai
{
    public Dictionary<int, Item> BanzaiTiles { get; set; }

    private Room _roomInstance;
    private bool _banzaiStarted;
    private byte[,] _floorMap;
    private GameField _field;
    private int _tilesUsed;

    public BattleBanzai(Room room)
    {
        this.BanzaiTiles = new Dictionary<int, Item>();

        this._roomInstance = room;
        this._banzaiStarted = false;
        this._tilesUsed = 0;
    }

    public void AddTile(Item item, int itemID)
    {
        if (this.BanzaiTiles.ContainsKey(itemID))
        {
            return;
        }

        this.BanzaiTiles.Add(itemID, item);
    }

    public void RemoveTile(int itemID) => this.BanzaiTiles.Remove(itemID);

    public void OnUserWalk(RoomUser user)
    {
        if (user == null)
        {
            return;
        }

        var roomItemForSquare = this._roomInstance.GameMap.GetCoordinatedItems(new Point(user.SetX, user.SetY));

        foreach (var ball in roomItemForSquare)
        {
            if (ball.GetBaseItem().InteractionType != InteractionType.BANZAIPUCK)
            {
                continue;
            }

            var lenght = 1;
            var goalX = ball.X;
            var goalY = ball.Y;

            switch (user.RotBody)
            {
                case 0:
                    goalX = ball.X;
                    goalY = ball.Y - lenght;
                    break;
                case 1:
                    goalX = ball.X + lenght;
                    goalY = ball.Y - lenght;
                    break;
                case 2:
                    goalX = ball.X + lenght;
                    goalY = ball.Y;
                    break;
                case 3:
                    goalX = ball.X + lenght;
                    goalY = ball.Y + lenght;
                    break;
                case 4:
                    goalX = ball.X;
                    goalY = ball.Y + lenght;
                    break;
                case 5:
                    goalX = ball.X - lenght;
                    goalY = ball.Y + lenght;
                    break;
                case 6:
                    goalX = ball.X - lenght;
                    goalY = ball.Y;
                    break;
                case 7:
                    goalX = ball.X - lenght;
                    goalY = ball.Y - lenght;
                    break;
            }

            if (!this._roomInstance.GameMap.CanStackItem(goalX, goalY))
            {
                switch (user.RotBody)
                {
                    case 0:
                        goalX = ball.X;
                        goalY = ball.Y + lenght;
                        break;
                    case 1:
                        goalX = ball.X - lenght;
                        goalY = ball.Y + lenght;
                        break;
                    case 2:
                        goalX = ball.X - lenght;
                        goalY = ball.Y;
                        break;
                    case 3:
                        goalX = ball.X - lenght;
                        goalY = ball.Y - lenght;
                        break;
                    case 4:
                        goalX = ball.X;
                        goalY = ball.Y - lenght;
                        break;
                    case 5:
                        goalX = ball.X + lenght;
                        goalY = ball.Y - lenght;
                        break;
                    case 6:
                        goalX = ball.X + lenght;
                        goalY = ball.Y;
                        break;
                    case 7:
                        goalX = ball.X + lenght;
                        goalY = ball.Y + lenght;
                        break;
                }
            }
            this.MovePuck(ball, user.Client, goalX, goalY, user.Team);
            break;
        }
    }

    public void BanzaiStart()
    {
        if (this._banzaiStarted)
        {
            return;
        }

        this._banzaiStarted = true;

        this._roomInstance.GameItemHandler.ResetAllBlob();
        this._roomInstance.GameManager.Reset();
        this._floorMap = new byte[this._roomInstance.GameMap.Model.MapSizeY, this._roomInstance.GameMap.Model.MapSizeX];
        this._field = new GameField(this._floorMap, true);

        for (var index = 1; index < 5; ++index)
        {
            this._roomInstance.GameManager.Points[index] = 0;
        }

        foreach (Item roomItem in (IEnumerable)this.BanzaiTiles.Values)
        {
            roomItem.ExtraData = "1";
            roomItem.Value = 0;
            roomItem.Team = TeamType.None;
            roomItem.UpdateState();
        }

        this._tilesUsed = 0;
    }

    public void BanzaiEnd()
    {
        if (!this._banzaiStarted)
        {
            return;
        }

        this._banzaiStarted = false;

        this._field.Destroy();

        if (this.BanzaiTiles.Count == 0)
        {
            return;
        }

        var winningTeam = this._roomInstance.GameManager.GetWinningTeam();

        foreach (var user in this._roomInstance.TeamManager.GetAllPlayer())
        {
            this.EndGame(user, winningTeam);
        }
    }

    private void EndGame(RoomUser roomUser, TeamType winningTeam)
    {
        if (roomUser.Team == winningTeam && winningTeam != TeamType.None)
        {
            this._roomInstance.SendPacket(new ActionComposer(roomUser.VirtualId, 1));
        }
        else if (roomUser.Team != TeamType.None)
        {
            var firstTile = this.GetFirstTile(roomUser.X, roomUser.Y);

            if (firstTile == null)
            {
                return;
            }

            if (this._roomInstance.GameItemHandler.GetExitTeleport() != null)
            {
                GameMap.TeleportToItem(roomUser, this._roomInstance.GameItemHandler.GetExitTeleport());
            }

            var managerForBanzai = roomUser.Client.User.CurrentRoom.TeamManager;
            managerForBanzai.OnUserLeave(roomUser);
            this._roomInstance.GameManager.UpdateGatesTeamCounts();
            roomUser.ApplyEffect(0);
            roomUser.Team = TeamType.None;

            roomUser.Client.SendPacket(new IsPlayingComposer(false));
        }
    }

    public void MovePuck(Item item, GameClient mover, int newX, int newY, TeamType team)
    {
        if (item == null || mover == null || !this._roomInstance.GameMap.CanStackItem(newX, newY))
        {
            return;
        }

        item.ExtraData = team.ToString();
        item.UpdateState();

        var oldZ = item.Z;
        var newZ = (double)this._roomInstance.GameMap.SqAbsoluteHeight(newX, newY);
        if (this._roomInstance.RoomItemHandling.SetFloorItem(item, newX, newY, newZ))
        {
            this._roomInstance.SendPacket(new SlideObjectBundleComposer(item.Coordinate.X, item.Coordinate.Y, oldZ, newX, newY, newZ, item.Id));
        }

        if (!this._banzaiStarted)
        {
            return;
        }

        this.HandleBanzaiTiles(new Point(newX, newY), team, this._roomInstance.RoomUserManager.GetRoomUserByUserId(mover.User.Id));
    }

    private void SetTile(Item item, TeamType team, RoomUser user)
    {
        if (item.Team == team)
        {
            if (item.Value < 3)
            {
                ++item.Value;
                if (item.Value == 3)
                {
                    this._roomInstance.GameManager.AddPointToTeam(item.Team, user);
                    this._field.UpdateLocation(item.X, item.Y, (byte)team);
                    foreach (var pointField in this._field.DoUpdate())
                    {
                        var team1 = (TeamType)pointField.ForValue;
                        foreach (var point in pointField.GetPoints())
                        {
                            if (this._floorMap[point.Y, point.X] == (byte)team1)
                            {
                                continue;
                            }

                            this.HandleMaxBanzaiTiles(new Point(point.X, point.Y), team1, user, (TeamType)this._floorMap[point.Y, point.X]);
                            this._floorMap[point.Y, point.X] = pointField.ForValue;
                        }
                    }
                    this._tilesUsed++;
                }
            }
        }
        else if (item.Value < 3)
        {
            item.Team = team;
            item.Value = 1;
        }
        var num = item.Value + ((int)item.Team * 3) - 1;
        item.ExtraData = num.ToString();
    }

    private Item GetFirstTile(int x, int y)
    {
        foreach (var roomItem in this._roomInstance.GameMap.GetCoordinatedItems(new Point(x, y)))
        {
            if (roomItem.GetBaseItem().InteractionType == InteractionType.BANZAIFLOOR)
            {
                return roomItem;
            }
        }
        return null;
    }

    public void HandleBanzaiTiles(Point coord, TeamType team, RoomUser user)
    {
        if (!this._banzaiStarted || team == TeamType.None || this.BanzaiTiles.Count == 0)
        {
            return;
        }

        var roomItem = this.GetFirstTile(coord.X, coord.Y);
        if (roomItem == null)
        {
            return;
        }

        if (roomItem.Value == 3)
        {
            return;
        }

        this.SetTile(roomItem, team, user);
        roomItem.UpdateState(false, true);

        if (this._tilesUsed != this.BanzaiTiles.Count)
        {
            return;
        }

        this.BanzaiEnd();
    }

    private void HandleMaxBanzaiTiles(Point coord, TeamType team, RoomUser user, TeamType oldteam)
    {
        if (team == TeamType.None)
        {
            return;
        }

        var roomItem = this.GetFirstTile(coord.X, coord.Y);
        if (roomItem == null)
        {
            return;
        }

        if (roomItem.Value != 3)
        {
            this._tilesUsed++;
        }

        SetMaxForTile(roomItem, team);
        this._roomInstance.GameManager.AddPointToTeam(team, user);
        this._roomInstance.GameManager.AddPointToTeam(oldteam, -1, user);
        roomItem.UpdateState(false, true);
    }

    private static void SetMaxForTile(Item item, TeamType team)
    {
        if (item.Value < 3)
        {
            item.Value = 3;
        }

        item.Team = team;
        var num = item.Value + ((int)item.Team * 3) - 1;
        item.ExtraData = num.ToString();
    }

    public void Destroy()
    {
        this.BanzaiTiles.Clear();
        Array.Clear(this._floorMap, 0, this._floorMap.Length);
        this._field.Destroy();
        this._roomInstance = null;
        this.BanzaiTiles = null;
        this._floorMap = null;
        this._field = null;
    }
}
