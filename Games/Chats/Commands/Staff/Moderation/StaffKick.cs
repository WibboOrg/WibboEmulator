namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
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

        var targetUser = GameClientManager.GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.User == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", session.Language));
        }
        else if (session.User.Rank <= targetUser.User.Rank)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("action.notallowed", session.Language));
        }
        else if (!targetUser.User.InRoom)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("kick.error", session.Language));
        }
        else
        {
            room.RoomUserManager.RemoveUserFromRoom(targetUser, true, false);

            if (parameters.Length > 2)
            {
                var message = CommandManager.MergeParams(parameters, 2);
                if (session.User.CheckChatMessage(message, "<CMD>", room.Id))
                {
                    return;
                }

                targetUser.SendNotification(LanguageManager.TryGetValue("kick.withmessage", targetUser.Language) + message);
            }
            else
            {
                targetUser.SendNotification(LanguageManager.TryGetValue("kick.nomessage", targetUser.Language));
            }
        }
    }
}
