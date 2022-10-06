namespace WibboEmulator.Games.Rooms.Games.Football;
using System.Drawing;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map.Movement;

public class Soccer
{
    private Room _roomInstance;

    public Soccer(Room room) => this._roomInstance = room;

    public void HandleFootballGameItems(Point ballItemCoord)
    {
        foreach (var roomItem in this._roomInstance.GetGameManager().GetItems(TeamType.RED).Values)
        {
            foreach (var threeDcoord in roomItem.GetAffectedTiles.Values)
            {
                if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                {
                    this.AddPointToScoreCounters(TeamType.RED);
                    return;
                }
            }
        }
        foreach (var roomItem in this._roomInstance.GetGameManager().GetItems(TeamType.GREEN).Values)
        {
            foreach (var threeDcoord in roomItem.GetAffectedTiles.Values)
            {
                if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                {
                    this.AddPointToScoreCounters(TeamType.GREEN);
                    return;
                }
            }
        }
        foreach (var roomItem in this._roomInstance.GetGameManager().GetItems(TeamType.BLUE).Values)
        {
            foreach (var threeDcoord in roomItem.GetAffectedTiles.Values)
            {
                if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                {
                    this.AddPointToScoreCounters(TeamType.BLUE);
                    return;
                }
            }
        }
        foreach (var roomItem in this._roomInstance.GetGameManager().GetItems(TeamType.YELLOW).Values)
        {
            foreach (var threeDcoord in roomItem.GetAffectedTiles.Values)
            {
                if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                {
                    this.AddPointToScoreCounters(TeamType.YELLOW);
                    return;
                }
            }
        }
    }

    private void AddPointToScoreCounters(TeamType team)
    {
        foreach (var roomItem in this._roomInstance.GetGameManager().GetItems(team).Values)
        {
            switch (roomItem.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALLCOUNTERBLUE:
                case InteractionType.FOOTBALLCOUNTERGREEN:
                case InteractionType.FOOTBALLCOUNTERRED:
                case InteractionType.FOOTBALLCOUNTERYELLOW:
                    _ = int.TryParse(roomItem.ExtraData, out var num);
                    num++;
                    if (num >= 100)
                    {
                        num = 0;
                    }

                    roomItem.ExtraData = num.ToString();
                    roomItem.UpdateState(false, true);
                    break;
            }
        }
    }

    public void OnUserWalk(RoomUser User, bool Shoot)
    {
        if (User == null)
        {
            return;
        }

        if (!User.AllowBall && Shoot && !this._roomInstance.OldFoot)
        {
            User.AllowBall = true;
            User.MoveWithBall = false;
            return;
        }

        var roomItemForSquare = this._roomInstance.GetGameMap().GetCoordinatedItems(new Point(User.SetX, User.SetY));

        var MoveBall = false;

        foreach (var Ball in roomItemForSquare)
        {
            if (Ball.GetBaseItem().InteractionType != InteractionType.FOOTBALL)
            {
                continue;
            }

            switch (User.RotBody)
            {
                case 0:
                    Ball.MovementDir = MovementDirection.up;
                    break;
                case 1:
                    Ball.MovementDir = MovementDirection.upright;
                    break;
                case 2:
                    Ball.MovementDir = MovementDirection.right;
                    break;
                case 3:
                    Ball.MovementDir = MovementDirection.downright;
                    break;
                case 4:
                    Ball.MovementDir = MovementDirection.down;
                    break;
                case 5:
                    Ball.MovementDir = MovementDirection.downleft;
                    break;
                case 6:
                    Ball.MovementDir = MovementDirection.left;
                    break;
                case 7:
                    Ball.MovementDir = MovementDirection.upleft;
                    break;
            }

            if (Shoot)
            {
                Ball.InteractionCountHelper = 6;
                Ball.InteractingUser = User.VirtualId;
                Ball.ReqUpdate(1);
            }
            else
            {
                var GoalX = Ball.X;
                var GoalY = Ball.Y;

                var NewPoint = Ball.GetMoveCoord(GoalX, GoalY, 1);

                Ball.InteractionCountHelper = 0;

                if (User.AllowBall && !User.MoveWithBall)
                {
                    User.AllowBall = false;
                }
                else
                {
                    User.AllowBall = true;
                }

                if (Ball.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
                {
                    this.MoveBall(Ball, NewPoint.X, NewPoint.Y);
                }
            }

            MoveBall = true;

            break;
        }

        if (!MoveBall)
        {
            User.SetMoveWithBall = true;
        }
        else
        {
            User.MoveWithBall = true;
        }
    }

    public void MoveBall(Item item, int newX, int newY)
    {
        item.ExtraData = (item.Value + 11).ToString();
        item.Value++;
        if (item.Value == 2)
        {
            item.Value = 0;
        }

        item.UpdateState(false, true);

        var Z = this._roomInstance.GetGameMap().SqAbsoluteHeight(newX, newY);
        this._roomInstance.GetRoomItemHandler().PositionReset(item, newX, newY, Z);

        this.HandleFootballGameItems(new Point(newX, newY));
    }

    public void Destroy() => this._roomInstance = null;
}
