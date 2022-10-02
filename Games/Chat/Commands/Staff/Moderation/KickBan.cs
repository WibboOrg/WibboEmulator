using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class KickBan : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            GameClient TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetUser().Rank <= TargetUser.GetUser().Rank)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
            }
            else if (TargetUser.GetUser().CurrentRoomId <= 0)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.error", Session.Langue));
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
