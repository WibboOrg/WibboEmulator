using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Game.Rooms;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class ForceSit : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            if (User == null)
                return;

            if (User.Statusses.ContainsKey("lie") || User.IsLay || User.RidingHorse || User.IsWalking || User.IsSit)
                return;

            if (!User.Statusses.ContainsKey("sit"))
            {
                if ((User.RotBody % 2) == 0)
                {
                    if (User == null)
                        return;

                    try
                    {
                        User.Statusses.Add("sit", "1.0");
                        User.Z -= 0.35;
                        User.IsSit = true;
                        User.UpdateNeeded = true;
                    }
                    catch { }
                }
                else
                {
                    User.RotBody--;
                    User.Statusses.Add("sit", "1.0");
                    User.Z -= 0.35;
                    User.IsSit = true;
                    User.UpdateNeeded = true;
                }
            }
            else if (User.IsSit == true)
            {
                User.Z += 0.35;
                User.Statusses.Remove("sit");
                User.Statusses.Remove("1.0");
                User.IsSit = false;
                User.UpdateNeeded = true;
            }
        }
    }
}