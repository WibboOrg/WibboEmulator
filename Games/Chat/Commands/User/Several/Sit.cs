using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Sit : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
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
}
