namespace WibboEmulator.Games.Chat.Commands.User.RP;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;

internal class GunFire : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (!userRoom.AllowShoot || userRoom.Freeze)
        {
            return;
        }

        var movement = MovementDirection.none;

        switch (userRoom.RotBody)
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

        if (userRoom.FreezeEndCounter <= 2)
        {
            userRoom.Freeze = true;
            userRoom.FreezeEndCounter = 2;
        }

        room.ProjectileManager.AddProjectile(userRoom.VirtualId, userRoom.SetX, userRoom.SetY, userRoom.SetZ, movement);
    }
}
