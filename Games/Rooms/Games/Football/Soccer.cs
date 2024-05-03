namespace WibboEmulator.Games.Rooms.Games.Football;
using System.Drawing;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map.Movement;

public class Soccer(Room room)
{
    public void HandleFootballGameItems(Point ballItemCoord)
    {
        foreach (var roomItem in room.GameManager.GetItems(TeamType.Red).Values)
        {
            foreach (var threeDcoord in roomItem.GetAffectedTiles)
            {
                if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                {
                    this.AddPointToScoreCounters(TeamType.Red);
                    return;
                }
            }
        }
        foreach (var roomItem in room.GameManager.GetItems(TeamType.Green).Values)
        {
            foreach (var threeDcoord in roomItem.GetAffectedTiles)
            {
                if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                {
                    this.AddPointToScoreCounters(TeamType.Green);
                    return;
                }
            }
        }
        foreach (var roomItem in room.GameManager.GetItems(TeamType.Blue).Values)
        {
            foreach (var threeDcoord in roomItem.GetAffectedTiles)
            {
                if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                {
                    this.AddPointToScoreCounters(TeamType.Blue);
                    return;
                }
            }
        }
        foreach (var roomItem in room.GameManager.GetItems(TeamType.Yellow).Values)
        {
            foreach (var threeDcoord in roomItem.GetAffectedTiles)
            {
                if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                {
                    this.AddPointToScoreCounters(TeamType.Yellow);
                    return;
                }
            }
        }
    }

    private void AddPointToScoreCounters(TeamType team)
    {
        foreach (var roomItem in room.GameManager.GetItems(team).Values)
        {
            switch (roomItem.ItemData.InteractionType)
            {
                case InteractionType.FOOTBALL_COUNTER_BLUE:
                case InteractionType.FOOTBALL_COUNTER_GREEN:
                case InteractionType.FOOTBALL_COUNTER_RED:
                case InteractionType.FOOTBALL_COUNTER_YELLOW:
                    _ = int.TryParse(roomItem.ExtraData, out var num);
                    num++;
                    if (num >= 100)
                    {
                        num = 0;
                    }

                    roomItem.ExtraData = num.ToString();
                    roomItem.UpdateState(false);
                    break;
            }
        }
    }

    public void OnUserWalk(RoomUser user, bool shoot)
    {
        if (user == null)
        {
            return;
        }

        if (!user.AllowBall && shoot && !room.OldFoot)
        {
            user.AllowBall = true;
            user.MoveWithBall = false;
            return;
        }

        var roomItemForSquare = room.GameMap.GetCoordinatedItems(new Point(user.SetX, user.SetY));

        var moveBall = false;

        foreach (var ball in roomItemForSquare)
        {
            if (ball.ItemData.InteractionType != InteractionType.FOOTBALL)
            {
                continue;
            }

            switch (user.RotBody)
            {
                case 0:
                    ball.MovementDir = MovementDirection.up;
                    break;
                case 1:
                    ball.MovementDir = MovementDirection.upright;
                    break;
                case 2:
                    ball.MovementDir = MovementDirection.right;
                    break;
                case 3:
                    ball.MovementDir = MovementDirection.downright;
                    break;
                case 4:
                    ball.MovementDir = MovementDirection.down;
                    break;
                case 5:
                    ball.MovementDir = MovementDirection.downleft;
                    break;
                case 6:
                    ball.MovementDir = MovementDirection.left;
                    break;
                case 7:
                    ball.MovementDir = MovementDirection.upleft;
                    break;
            }

            if (shoot)
            {
                ball.InteractionCountHelper = 6;
                ball.InteractingUser = user.VirtualId;
                ball.ReqUpdate(1);
            }
            else
            {
                var goalX = ball.X;
                var goalY = ball.Y;

                var newPoint = ball.GetMoveCoord(goalX, goalY, 1);

                ball.InteractionCountHelper = 0;

                if (user.AllowBall && !user.MoveWithBall)
                {
                    user.AllowBall = false;
                }
                else
                {
                    user.AllowBall = true;
                }

                if (ball.Room.GameMap.CanStackItem(newPoint.X, newPoint.Y, true))
                {
                    this.MoveBall(ball, newPoint.X, newPoint.Y);
                }
            }

            moveBall = true;

            break;
        }

        if (!moveBall)
        {
            user.SetMoveWithBall = true;
        }
        else
        {
            user.MoveWithBall = true;
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

        item.UpdateState(false);

        var z = room.GameMap.SqAbsoluteHeight(newX, newY);
        room.RoomItemHandling.PositionReset(item, newX, newY, z);

        this.HandleFootballGameItems(new Point(newX, newY));
    }

    public void Destroy() => room = null;
}
