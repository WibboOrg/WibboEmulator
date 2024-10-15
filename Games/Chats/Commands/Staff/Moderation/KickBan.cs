namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class KickBan : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var TargetUser = GameClientManager.GetClientByUsername(parameters[1]);
        if (TargetUser == null || TargetUser.User == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", Session.Language));
        }
        else if (Session.User.Rank <= TargetUser.User.Rank)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("action.notallowed", Session.Language));
        }
        else if (!TargetUser.User.InRoom)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("kick.error", Session.Language));
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

            var expireTime = WibboEnvironment.GetUnixTimestamp() + (banMinutes * 60);

            room.AddBan(TargetUser.User.Id, expireTime);
            room.RoomUserManager.RemoveUserFromRoom(TargetUser, true, true);
        }
    }
}
