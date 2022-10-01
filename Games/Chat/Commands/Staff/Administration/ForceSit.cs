using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ForceSit : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            if (User == null)
                return;

            if (User.ContainStatus("lay") || User.IsLay || User.RidingHorse || User.IsWalking || User.IsSit)
                return;

            if (!User.ContainStatus("sit"))
            {
                if ((User.RotBody % 2) == 0)
                {
                    if (User == null)
                        return;

                    try
                    {
                        User.SetStatus("sit", "1.0");
                        User.Z -= 0.35;
                        User.IsSit = true;
                        User.UpdateNeeded = true;
                    }
                    catch { }
                }
                else
                {
                    User.RotBody--;
                    User.SetStatus("sit", "1.0");
                    User.Z -= 0.35;
                    User.IsSit = true;
                    User.UpdateNeeded = true;
                }
            }
            else if (User.IsSit == true)
            {
                User.Z += 0.35;
                User.RemoveStatus("sit");
                User.IsSit = false;
                User.UpdateNeeded = true;
            }
        }
    }
}