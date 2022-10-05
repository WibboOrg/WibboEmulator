namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Lay : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (UserRoom.ContainStatus("lay") || UserRoom.ContainStatus("sit"))
        {
            return;
        }

        if (UserRoom.RotBody % 2 == 0 || UserRoom.IsTransf)
        {
            if (UserRoom.RotBody == 4 || UserRoom.RotBody == 0 || UserRoom.IsTransf)
            {
                if (Room.GetGameMap().CanWalk(UserRoom.X, UserRoom.Y + 1))
                {
                    UserRoom.RotBody = 0;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (!Room.GetGameMap().CanWalk(UserRoom.X + 1, UserRoom.Y))
                {
                    return;
                }
            }

            if (UserRoom.IsTransf)
            {
                UserRoom.SetStatus("lay", "0");
            }
            else
            {
                UserRoom.SetStatus("lay", "0.7");
            }

            UserRoom.IsLay = true;
            UserRoom.UpdateNeeded = true;
        }
    }
}
