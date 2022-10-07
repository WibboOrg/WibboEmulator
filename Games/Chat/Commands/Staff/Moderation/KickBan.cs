namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class KickBan : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.GetUser() == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
        else if (session.GetUser().Rank <= targetUser.GetUser().Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
        }
        else if (targetUser.GetUser().CurrentRoomId <= 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.error", session.Langue));
        }
        else
        {
            var banMinutes = 2;

            if (parameters.Length >= 3)
            {
                _ = int.TryParse(parameters[2], out banMinutes);
            }

            if (banMinutes <= 0)
            {
                banMinutes = 2;
            }

            room.AddBan(targetUser.GetUser().Id, banMinutes * 60);
            room.GetRoomUserManager().RemoveUserFromRoom(targetUser, true, true);
        }
    }
}
