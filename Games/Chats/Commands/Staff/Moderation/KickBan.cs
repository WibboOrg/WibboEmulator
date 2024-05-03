namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class KickBan : IChatCommand
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
            var banMinutes = 2;

            if (parameters.Length >= 3)
            {
                _ = int.TryParse(parameters[2], out banMinutes);
            }

            if (banMinutes <= 0)
            {
                banMinutes = 2;
            }

            room.AddBan(targetUser.User.Id, banMinutes * 60);
            room.RoomUserManager.RemoveUserFromRoom(targetUser, true, true);
        }
    }
}
