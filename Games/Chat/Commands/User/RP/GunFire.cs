using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms.Map.Movement;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class GunFire : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!UserRoom.AllowShoot || UserRoom.Freeze)
            {
                return;
            }

            MovementDirection movement = MovementDirection.none;

            switch (UserRoom.RotBody)
            {
                case 0:
                    movement = MovementDirection.up;
                    break;
                case 1:
                    movement = MovementDirection.upright;
                    break;
                case 2:
                    movement = MovementDirection.right;
                    break;
                case 3:
                    movement = MovementDirection.downright;
                    break;
                case 4:
                    movement = MovementDirection.down;
                    break;
                case 5:
                    movement = MovementDirection.downleft;
                    break;
                case 6:
                    movement = MovementDirection.left;
                    break;
                case 7:
                    movement = MovementDirection.upleft;
                    break;
            }

            if (UserRoom.FreezeEndCounter <= 2)
            {
                UserRoom.Freeze = true;
                UserRoom.FreezeEndCounter = 2;
            }

            Room.GetProjectileManager().AddProjectile(UserRoom.VirtualId, UserRoom.SetX, UserRoom.SetY, UserRoom.SetZ, movement);
        }
    }
}
