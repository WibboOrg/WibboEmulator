namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class StaffKick : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.User == null)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
        else if (session.User.Rank <= targetUser.User.Rank)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
        }
        else if (!targetUser.User.InRoom)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("kick.error", session.Langue));
        }
        else
        {
            room.RoomUserManager.RemoveUserFromRoom(targetUser, true, false);

            if (parameters.Length > 2)
            {
                var message = CommandManager.MergeParams(parameters, 2);
                if (session.User.Antipub(message, "<CMD>", room.Id))
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
