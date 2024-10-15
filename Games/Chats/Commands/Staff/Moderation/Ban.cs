namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Ban : IChatCommand
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

        _ = int.TryParse(parameters.Length >= 3 ? parameters[2] : "0", out var expire);
        if (expire <= 600)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("ban.toolesstime", Session.Language));
        }
        else
        {
            var raison = CommandManager.MergeParams(parameters, 3);
            Session.SendWhisper("Tu as bannit " + TargetUser.User.Username + " pour " + raison + "!");

            GameClientManager.BanUser(TargetUser, Session.User.Username, expire, raison, false);
            _ = Session.User.CheckChatMessage(raison, "<CMD>", room.Id);
        }
    }
}
