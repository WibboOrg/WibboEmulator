namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class StaffKick : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser user, string[] parameters)
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
        else if (targetUser.GetUser().CurrentRoomId < 1U)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.error", session.Langue));
        }
        else
        {
            room.RoomUserManager.RemoveUserFromRoom(targetUser, true, false);

            if (parameters.Length > 2)
            {
                var message = CommandManager.MergeParams(parameters, 2);
                if (session.Antipub(message, "<CMD>", room.Id))
                {
                    return;
                }

                targetUser.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.withmessage", targetUser.Langue) + message);
            }
            else
            {
                targetUser.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.nomessage", targetUser.Langue));
            }
        }
    }
}
