using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Rooms.Map.Movement;
using System.Drawing;

namespace WibboEmulator.Game.Items.Interactors
{
    public class InteractorFootball : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Ball, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null)
            {
                return;
            }

            RoomUser User = Ball.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);

            if (User == null)
            {
                return;
            }

            bool TropLoin = true;
            bool Deloin = false;

            int differenceX = User.SetX - Ball.X;
            int differenceY = User.SetY - Ball.Y;

            if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1)
            {
                TropLoin = false;
            }

            int differenceX2 = User.X - Ball.X;
            int differenceY2 = User.Y - Ball.Y;

            if (differenceX2 <= 1 && differenceX2 >= -1 && differenceY2 <= 1 && differenceY2 >= -1)
            {
                TropLoin = false;
            }

            int differenceX3 = User.GoalX - Ball.X;
            int differenceY3 = User.GoalY - Ball.Y;

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


            int GoalX = Ball.X;
            int GoalY = Ball.Y;

            Point NewPoint = Ball.GetMoveCoord(GoalX, GoalY, 1);

            Ball.InteractionCountHelper = 0;

            if (!Ball.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
            {
                Ball.GetNewDir(NewPoint.X, NewPoint.Y);
                NewPoint = Ball.GetMoveCoord(GoalX, GoalY, 1);
            }

            if (Ball.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
            {
                Ball.GetRoom().GetSoccer().MoveBall(Ball, NewPoint.X, NewPoint.Y);
            }

            if (!User.MoveWithBall && !Deloin && Ball.InteractionCountHelper == 0 && !Ball.GetRoom().OldFoot)
            {
                Ball.InteractionCountHelper = 2;
                Ball.InteractingUser = User.VirtualId;
                Ball.ReqUpdate(1);
            }
        }

        public override void OnTick(Item item)
        {
            if (item.InteractionCountHelper <= 0 || item.InteractionCountHelper > 6)
            {
                item.ExtraData = "0";
                item.UpdateState(false, true);

                item.InteractionCountHelper = 0;
                return;
            }

            int OldX = item.X;
            int OldY = item.Y;

            int NewX = item.X;
            int NewY = item.Y;

            Point NewPoint = item.GetMoveCoord(OldX, OldY, 1);

            int Length;
            if (item.InteractionCountHelper > 3)
            {
                Length = 3;

                item.ExtraData = "6";
                item.UpdateState(false, true);
            }
            else if (item.InteractionCountHelper > 1 && item.InteractionCountHelper < 4)
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

            for (int i = 1; i <= Length; i++)
            {
                NewPoint = item.GetMoveCoord(OldX, OldY, i);

                if ((item.InteractionCountHelper <= 3 && item.GetRoom().GetGameMap().SquareHasUsers(NewPoint.X, NewPoint.Y)))
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
                    List<RoomUser> Users = item.GetRoom().GetGameMap().GetNearUsers(new Point(NewPoint.X, NewPoint.Y), 1);
                    if (Users != null)
                    {
                        bool BreakMe = false;
                        foreach (RoomUser User in Users)
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

            double Z = item.GetRoom().GetGameMap().SqAbsoluteHeight(NewX, NewY);
            item.GetRoom().GetRoomItemHandler().PositionReset(item, NewX, NewY, Z);

            item.UpdateCounter = 1;
        }
    }
}
