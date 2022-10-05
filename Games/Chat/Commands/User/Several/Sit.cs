namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Sit : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (UserRoom.ContainStatus("sit") || UserRoom.ContainStatus("lay"))
        {
            return;
        }

        if (UserRoom.RotBody % 2 == 0)
        {
            if (UserRoom.IsTransf)
            {
                UserRoom.SetStatus("sit", "0");
            }
            else
            {
                UserRoom.SetStatus("sit", "0.5");
            }

            UserRoom.IsSit = true;
            UserRoom.UpdateNeeded = true;
        }
    }
}
