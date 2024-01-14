namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
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

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.User == null)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        if (targetUser.User.Rank >= session.User.Rank)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
            return;
        }

        _ = int.TryParse(parameters.Length >= 3 ? parameters[2] : "0", out var expire);
        if (expire <= 600)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", session.Langue));
        }
        else
        {
            var raison = CommandManager.MergeParams(parameters, 3);
            session.SendWhisper("Tu as bannit " + targetUser.User.Username + " pour " + raison + "!");

            WibboEnvironment.GetGame().GetGameClientManager().BanUser(targetUser, session.User.Username, expire, raison, false);
            _ = session.User.CheckChatMessage(raison, "<CMD>", room.Id);
        }
    }
}
