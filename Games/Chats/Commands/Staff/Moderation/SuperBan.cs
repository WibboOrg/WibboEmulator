namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SuperBan : IChatCommand
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
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        if (targetUser.User.Rank >= session.User.Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
            WibboEnvironment.GetGame().GetGameClientManager().BanUser(session, "Robot", 788922000, "Votre compte a été banni par sécurité !", false, false);
        }
        else
        {
            var num = 788922000;
            if (parameters.Length >= 3)
            {
                _ = int.TryParse(parameters[2], out num);
            }

            if (num <= 600)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", session.Langue));
            }
            else
            {
                var raison = CommandManager.MergeParams(parameters, 3);
                session.SendWhisper("Tu as SuperBan " + targetUser.User.Username + " pour" + raison + "!");

                WibboEnvironment.GetGame().GetGameClientManager().BanUser(targetUser, session.User.Username, num, raison, false, false);
                _ = session.Antipub(raison, "<CMD>", room.Id);
            }
        }
    }
}
