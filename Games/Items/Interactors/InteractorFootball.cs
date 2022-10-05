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

        var User = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);

        if (User == null)
        {
            return;
        }

        var TropLoin = true;
        var Deloin = false;

        var differenceX = User.SetX - item.X;
        var differenceY = User.SetY - item.Y;

        if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1)
        {
            TropLoin = false;
        }

        var differenceX2 = User.X - item.X;
        var differenceY2 = User.Y - item.Y;

        if (differenceX2 <= 1 && differenceX2 >= -1 && differenceY2 <= 1 && differenceY2 >= -1)
        {
            TropLoin = false;
        }

        var differenceX3 = User.GoalX - item.X;
        var differenceY3 = User.GoalY - item.Y;

        if (differenceX3 > 1 || differenceX3 < -1 || differenceY3 > 1 || differenceY3 < -1)
        {
            Deloin = true;
        }

        if (TropLoin)
        {
            return;
        }

        switch (User.RotHead)
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


        var GoalX = item.X;
        var GoalY = item.Y;

        var NewPoint = item.GetMoveCoord(GoalX, GoalY, 1);

        item.InteractionCountHelper = 0;

        if (!item.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
        {
            item.GetNewDir(NewPoint.X, NewPoint.Y);
            NewPoint = item.GetMoveCoord(GoalX, GoalY, 1);
        }

        if (item.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
        {
            item.GetRoom().GetSoccer().MoveBall(item, NewPoint.X, NewPoint.Y);
        }

        if (!User.MoveWithBall && !Deloin && item.InteractionCountHelper == 0 && !item.GetRoom().OldFoot)
        {
            item.InteractionCountHelper = 2;
            item.InteractingUser = User.VirtualId;
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

        var OldX = item.X;
        var OldY = item.Y;

        var NewX = item.X;
        var NewY = item.Y;

        var NewPoint = item.GetMoveCoord(OldX, OldY, 1);

        int Length;
        if (item.InteractionCountHelper > 3)
        {
            Length = 3;

            item.ExtraData = "6";
            item.UpdateState(false, true);
        }
        else if (item.InteractionCountHelper is > 1 and < 4)
        {
            Length = 2;

            item.ExtraData = "4";
            item.UpdateState(false, true);
        }
        else
        {
            Length = 1;

            item.ExtraData = "2";
            item.UpdateState(false, true);
        }


        if (Length != 1 && !item.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
        {
            item.GetNewDir(NewX, NewY);
            item.InteractionCountHelper--;
        }

        for (var i = 1; i <= Length; i++)
        {
            NewPoint = item.GetMoveCoord(OldX, OldY, i);

            if (item.InteractionCountHelper <= 3 && item.GetRoom().GetGameMap().SquareHasUsers(NewPoint.X, NewPoint.Y))
            {
                item.InteractionCountHelper = 0;
                break;
            }

            if (item.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
            {
                NewX = NewPoint.X;
                NewY = NewPoint.Y;
                item.GetRoom().GetSoccer().HandleFootballGameItems(new Point(NewPoint.X, NewPoint.Y));
            }
            else
            {
                item.GetNewDir(NewX, NewY);
                item.InteractionCountHelper--;
                break;
            }

            if (!item.GetRoom().GetGameMap().SquareTakingOpen(NewPoint.X, NewPoint.Y))
            {
                var Users = item.GetRoom().GetGameMap().GetNearUsers(new Point(NewPoint.X, NewPoint.Y), 1);
                if (Users != null)
                {
                    var BreakMe = false;
                    foreach (var User in Users)
                    {
                        if (User == null || item.InteractingUser == User.VirtualId)
                        {
                            continue;
                        }

                        if (User.SetX != NewPoint.X || User.SetY != NewPoint.Y)
                        {
                            continue;
                        }

                        if (User.SetStep && User.SetX == User.GoalX && User.SetY == User.GoalY)
                        {
                            item.InteractionCountHelper = 6;
                            item.InteractingUser = User.VirtualId;
                            item.MovementDir = MovementUtility.GetMovementByDirection(User.RotBody);
                            BreakMe = true;
                            break;
                        }
                    }

                    if (BreakMe)
                    {
                        return;
                    }
                }
            }

            item.InteractionCountHelper--;
        }

        var Z = item.GetRoom().GetGameMap().SqAbsoluteHeight(NewX, NewY);
        item.GetRoom().GetRoomItemHandler().PositionReset(item, NewX, NewY, Z);

        item.UpdateCounter = 1;
    }
}
