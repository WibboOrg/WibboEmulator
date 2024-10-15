namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Ban : IChatCommand
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
            return;
        }

        if (targetUser.User.Rank >= session.User.Rank)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("action.notallowed", session.Language));
            return;
        }

        _ = int.TryParse(parameters.Length >= 3 ? parameters[2] : "0", out var expire);
        if (expire <= 600)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("ban.toolesstime", session.Language));
        }
        else
        {
            var raison = CommandManager.MergeParams(parameters, 3);
            session.SendWhisper("Tu as bannit " + targetUser.User.Username + " pour " + raison + "!");

            GameClientManager.BanUser(targetUser, session.User.Username, expire, raison, false);
            _ = session.User.CheckChatMessage(raison, "<CMD>", room.Id);
        }
    }
}
