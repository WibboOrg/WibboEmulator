using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class KickBan : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetHabbo().Rank <= clientByUsername.GetHabbo().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
            }
            else if (clientByUsername.GetHabbo().CurrentRoomId <= 0)
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

                Room.AddBan(clientByUsername.GetHabbo().Id, banMinutes * 60);
                Room.GetRoomUserManager().RemoveUserFromRoom(clientByUsername, true, true);
            }
        }
    }
}
