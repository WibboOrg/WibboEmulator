namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Lay : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.ContainStatus("lay") || userRoom.ContainStatus("sit"))
        {
            return;
        }

        if (userRoom.RotBody % 2 == 0 || userRoom.IsTransf)
        {
            if (userRoom.RotBody == 4 || userRoom.RotBody == 0 || userRoom.IsTransf)
            {
                if (room.GetGameMap().CanWalk(userRoom.X, userRoom.Y + 1))
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
                if (!room.GetGameMap().CanWalk(userRoom.X + 1, userRoom.Y))
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
