namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
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
