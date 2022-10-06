namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Ban : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (TargetUser == null || TargetUser.GetUser() == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        if (TargetUser.GetUser().Rank >= session.GetUser().Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
            return;
        }

        int.TryParse(parameters.Length >= 3 ? parameters[2] : "0", out var num);
        if (num <= 600)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", session.Langue));
        }
        else
        {
            var Raison = CommandManager.MergeParams(parameters, 3);
            session.SendWhisper("Tu as bannit " + TargetUser.GetUser().Username + " pour " + Raison + "!");

            WibboEnvironment.GetGame().GetGameClientManager().BanUser(TargetUser, session.GetUser().Username, num, Raison, false, false);
            session.Antipub(Raison, "<CMD>", Room.Id);
        }
    }
}
