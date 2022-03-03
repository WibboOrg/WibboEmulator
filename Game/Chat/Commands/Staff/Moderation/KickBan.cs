using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class KickBan : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetUser().Rank <= TargetUser.GetUser().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
            }
            else if (TargetUser.GetUser().CurrentRoomId <= 0)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("kick.error", Session.Langue));
            }
            else
            {
                int banMinutes = 2;

                if (Params.Length >= 3)
                {
                    int.TryParse(Params[2], out banMinutes);
                }

                if (banMinutes <= 0)
                {
                    banMinutes = 2;
                }

                Room.AddBan(TargetUser.GetUser().Id, banMinutes * 60);
                Room.GetRoomUserManager().RemoveUserFromRoom(TargetUser, true, true);
            }
        }
    }
}
