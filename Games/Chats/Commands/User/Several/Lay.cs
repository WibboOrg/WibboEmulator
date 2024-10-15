namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Lay : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.ContainStatus("lay") || userRoom.ContainStatus("sit"))
        {
            return;
        }

        if (userRoom.RotBody % 2 == 0 || userRoom.IsTransf)
        {
            if (userRoom.RotBody == 4 || userRoom.RotBody == 0 || userRoom.IsTransf)
            {
                if (room.GameMap.CanWalk(userRoom.X, userRoom.Y + 1))
                {
                    userRoom.RotBody = 0;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (!room.GameMap.CanWalk(userRoom.X + 1, userRoom.Y))
                {
                    return;
                }
            }

            if (userRoom.IsTransf)
            {
                userRoom.SetStatus("lay", "0");
            }
            else
            {
                userRoom.SetStatus("lay", "0.7");
            }

            userRoom.IsLay = true;
            userRoom.UpdateNeeded = true;
        }
    }
}
