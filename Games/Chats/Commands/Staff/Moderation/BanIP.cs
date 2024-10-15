namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class BanIP : IChatCommand
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

        var reason = "";
        if (parameters.Length > 2)
        {
            reason = CommandManager.MergeParams(parameters, 2);
        }

        var securityBan = targetUser.User.Rank > 5 && session.User.Rank < 12;

        session.SendWhisper("Tu as banIP " + targetUser.User.Username + " pour " + reason + "!");

        GameClientManager.BanUser(targetUser, session.User.Username, -1, reason, true);
        _ = session.User.CheckChatMessage(reason, "<CMD>", room.Id);

        if (securityBan)
        {
            GameClientManager.BanUser(session, "Robot", -1, "Votre compte à été banni par sécurité", false);
        }
    }
}
