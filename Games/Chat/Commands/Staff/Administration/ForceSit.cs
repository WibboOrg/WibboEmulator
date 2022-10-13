namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceSit : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length == 1)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByName(parameters[1]);
        if (user == null)
        {
            return;
        }

        if (user.ContainStatus("lay") || user.IsLay || user.RidingHorse || user.IsWalking || user.IsSit)
        {
            return;
        }

        if (!user.ContainStatus("sit"))
        {
            if (user.RotBody % 2 == 0)
            {
                if (user == null)
                {
                    return;
                }

                try
                {
                    user.SetStatus("sit", "1.0");
                    user.Z -= 0.35;
                    user.IsSit = true;
                    user.UpdateNeeded = true;
                }
                catch { }
            }
            else
            {
                user.RotBody--;
                user.SetStatus("sit", "1.0");
                user.Z -= 0.35;
                user.IsSit = true;
                user.UpdateNeeded = true;
            }
        }
        else if (user.IsSit)
        {
            user.Z += 0.35;
            user.RemoveStatus("sit");
            user.IsSit = false;
            user.UpdateNeeded = true;
        }
    }
}
