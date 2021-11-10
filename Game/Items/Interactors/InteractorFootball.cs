using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Map.Movement;
using System.Drawing;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorFootball : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Ball, int Request, bool UserHasRights)
        {
            if (Session == null)
            {
                return;
            }

            RoomUser User = Ball.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            bool TropLoin = true;
            bool Deloin = false;

            int differenceX = User.SetX - Ball.GetX;
            int differenceY = User.SetY - Ball.GetY;

            if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1)
            {
                TropLoin = false;
            }

            int differenceX2 = User.X - Ball.GetX;
            int differenceY2 = User.Y - Ball.GetY;

            if (differenceX2 <= 1 && differenceX2 >= -1 && differenceY2 <= 1 && differenceY2 >= -1)
            {
                TropLoin = false;
            }

            int differenceX3 = User.GoalX - Ball.GetX;
            int differenceY3 = User.GoalY - Ball.GetY;

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


            int GoalX = Ball.GetX;
            int GoalY = Ball.GetY;

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
    }
}
