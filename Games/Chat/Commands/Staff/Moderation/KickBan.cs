namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class KickBan : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length < 2)
        {
            return;
        }

        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
        if (TargetUser == null || TargetUser.GetUser() == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
        else if (session.GetUser().Rank <= TargetUser.GetUser().Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
        }
        else if (TargetUser.GetUser().CurrentRoomId <= 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.error", session.Langue));
        }
        else
        {
            var banMinutes = 2;

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
