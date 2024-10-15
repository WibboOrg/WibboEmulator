namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class BanIP : IChatCommand
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
            return;
        }

        if (TargetUser.User.Rank >= Session.User.Rank)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("action.notallowed", Session.Language));
            return;
        }

        var reason = "";
        if (parameters.Length > 2)
        {
            reason = CommandManager.MergeParams(parameters, 2);
        }

        var securityBan = TargetUser.User.Rank > 5 && Session.User.Rank < 12;

        Session.SendWhisper("Tu as banIP " + TargetUser.User.Username + " pour " + reason + "!");

        GameClientManager.BanUser(TargetUser, Session.User.Username, -1, reason, true);
        _ = Session.User.CheckChatMessage(reason, "<CMD>", room.Id);

        if (securityBan)
        {
            GameClientManager.BanUser(Session, "Robot", -1, "Votre compte à été banni par sécurité", false);
        }
    }
}
