namespace WibboEmulator.Games.Items.Interactors;
using System.Drawing;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map.Movement;

public class InteractorFootball : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null)
        {
            return;
        }

        var user = item.GetRoom().RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);

        if (user == null)
        {
            return;
        }

        var tooLong = true;
        var fromAfar = false;

        var differenceX = user.SetX - item.X;
        var differenceY = user.SetY - item.Y;

        if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1)
        {
            tooLong = false;
        }

        var differenceX2 = user.X - item.X;
        var differenceY2 = user.Y - item.Y;

        if (differenceX2 <= 1 && differenceX2 >= -1 && differenceY2 <= 1 && differenceY2 >= -1)
        {
            tooLong = false;
        }

        var differenceX3 = user.GoalX - item.X;
        var differenceY3 = user.GoalY - item.Y;

        if (differenceX3 > 1 || differenceX3 < -1 || differenceY3 > 1 || differenceY3 < -1)
        {
            fromAfar = true;
        }

        if (tooLong)
        {
            return;
        }

        switch (user.RotHead)
        {
            case 0:
                item.MovementDir = MovementDirection.up;
                break;
            case 1:
                item.MovementDir = MovementDirection.upright;
                break;
            case 2:
                item.MovementDir = MovementDirection.right;
                break;
            case 3:
                item.MovementDir = MovementDirection.downright;
                break;
            case 4:
                item.MovementDir = MovementDirection.down;
                break;
            case 5:
                item.MovementDir = MovementDirection.downleft;
                break;
            case 6:
                item.MovementDir = MovementDirection.left;
                break;
            case 7:
                item.MovementDir = MovementDirection.upleft;
                break;
        }


        var goalX = item.X;
        var goalY = item.Y;

        var newPoint = item.GetMoveCoord(goalX, goalY, 1);

        item.InteractionCountHelper = 0;

        if (!item.GetRoom().GameMap.CanStackItem(newPoint.X, newPoint.Y, true))
        {
            item.GetNewDir(newPoint.X, newPoint.Y);
            newPoint = item.GetMoveCoord(goalX, goalY, 1);
        }

        if (item.GetRoom().GameMap.CanStackItem(newPoint.X, newPoint.Y, true))
        {
            item.GetRoom().Soccer.MoveBall(item, newPoint.X, newPoint.Y);
        }

        if (!user.MoveWithBall && !fromAfar && item.InteractionCountHelper == 0 && !item.GetRoom().OldFoot)
        {
            item.InteractionCountHelper = 2;
            item.InteractingUser = user.VirtualId;
            item.ReqUpdate(1);
        }
    }

    public override void OnTick(Item item)
    {
        if (item.InteractionCountHelper is <= 0 or > 6)
        {
            item.ExtraData = "0";
            item.UpdateState(false, true);

            item.InteractionCountHelper = 0;
            return;
        }

        var oldX = item.X;
        var oldY = item.Y;

        var newX = item.X;
        var newY = item.Y;

        var newPoint = item.GetMoveCoord(oldX, oldY, 1);

        int length;
        if (item.InteractionCountHelper > 3)
        {
            length = 3;

            item.ExtraData = "6";
            item.UpdateState(false, true);
        }
        else if (item.InteractionCountHelper is > 1 and < 4)
        {
            length = 2;

            item.ExtraData = "4";
            item.UpdateState(false, true);
        }
        else
        {
            length = 1;

            item.ExtraData = "2";
            item.UpdateState(false, true);
        }


        if (length != 1 && !item.GetRoom().GameMap.CanStackItem(newPoint.X, newPoint.Y, true))
        {
            item.GetNewDir(newX, newY);
            item.InteractionCountHelper--;
        }

        for (var i = 1; i <= length; i++)
        {
            newPoint = item.GetMoveCoord(oldX, oldY, i);

            if (item.InteractionCountHelper <= 3 && item.GetRoom().GameMap.SquareHasUsers(newPoint.X, newPoint.Y))
            {
                item.InteractionCountHelper = 0;
                break;
            }

            if (item.GetRoom().GameMap.CanStackItem(newPoint.X, newPoint.Y, true))
            {
                newX = newPoint.X;
                newY = newPoint.Y;
                item.GetRoom().Soccer.HandleFootballGameItems(new Point(newPoint.X, newPoint.Y));
            }
            else
            {
                item.GetNewDir(newX, newY);
                item.InteractionCountHelper--;
                break;
            }

            if (!item.GetRoom().GameMap.SquareTakingOpen(newPoint.X, newPoint.Y))
            {
                var users = item.GetRoom().GameMap.GetNearUsers(new Point(newPoint.X, newPoint.Y), 1);
                if (users != null)
                {
                    var breakMe = false;
                    foreach (var user in users)
                    {
                        if (user == null || item.InteractingUser == user.VirtualId)
                        {
                            continue;
                        }

                        if (user.SetX != newPoint.X || user.SetY != newPoint.Y)
                        {
                            continue;
                        }

                        if (user.SetStep && user.SetX == user.GoalX && user.SetY == user.GoalY)
                        {
                            item.InteractionCountHelper = 6;
                            item.InteractingUser = user.VirtualId;
                            item.MovementDir = MovementUtility.GetMovementByDirection(user.RotBody);
                            breakMe = true;
                            break;
                        }
                    }

                    if (breakMe)
                    {
                        return;
                    }
                }
            }

            item.InteractionCountHelper--;
        }

        var z = item.GetRoom().GameMap.SqAbsoluteHeight(newX, newY);
        item.GetRoom().RoomItemHandling.PositionReset(item, newX, newY, z);

        item.UpdateCounter = 1;
    }
}
