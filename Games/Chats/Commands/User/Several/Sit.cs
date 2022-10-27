namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Sit : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.ContainStatus("sit") || userRoom.ContainStatus("lay"))
        {
            return;
        }

        if (userRoom.RotBody % 2 == 0)
        {
            if (userRoom.IsTransf)
            {
                userRoom.SetStatus("sit", "0");
            }
            else
            {
                userRoom.SetStatus("sit", "0.5");
            }

            userRoom.IsSit = true;
            userRoom.UpdateNeeded = true;
        }
    }
}
