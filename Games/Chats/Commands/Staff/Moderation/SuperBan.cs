namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SuperBan : IChatCommand
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
            GameClientManager.BanUser(session, "Robot", -1, "Votre compte a été banni par sécurité !", false);
        }
        else
        {
            var num = -1;
            if (parameters.Length >= 3)
            {
                _ = int.TryParse(parameters[2], out num);
            }

            if (num is <= 600 and not (-1))
            {
                userRoom.SendWhisperChat(LanguageManager.TryGetValue("ban.toolesstime", session.Language));
            }
            else
            {
                var raison = CommandManager.MergeParams(parameters, 3);
                session.SendWhisper("Tu as SuperBan " + targetUser.User.Username + " pour" + raison + "!");

                GameClientManager.BanUser(targetUser, session.User.Username, num, raison, false);
                _ = session.User.CheckChatMessage(raison, "<CMD>", room.Id);
            }
        }
    }
}
