namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SuperBan : IChatCommand
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
            GameClientManager.BanUser(Session, "Robot", -1, "Votre compte a été banni par sécurité !", false);
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
                userRoom.SendWhisperChat(LanguageManager.TryGetValue("ban.toolesstime", Session.Language));
            }
            else
            {
                var raison = CommandManager.MergeParams(parameters, 3);
                Session.SendWhisper("Tu as SuperBan " + TargetUser.User.Username + " pour" + raison + "!");

                GameClientManager.BanUser(TargetUser, Session.User.Username, num, raison, false);
                _ = Session.User.CheckChatMessage(raison, "<CMD>", room.Id);
            }
        }
    }
}
