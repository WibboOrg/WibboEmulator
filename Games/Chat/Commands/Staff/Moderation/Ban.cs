namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Ban : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length < 2)
        {
            return;
        }

        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
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

        int.TryParse(Params.Length >= 3 ? Params[2] : "0", out var num);
        if (num <= 600)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", session.Langue));
        }
        else
        {
            var Raison = CommandManager.MergeParams(Params, 3);
            session.SendWhisper("Tu as bannit " + TargetUser.GetUser().Username + " pour " + Raison + "!");

            WibboEnvironment.GetGame().GetGameClientManager().BanUser(TargetUser, session.GetUser().Username, num, Raison, false, false);
            session.Antipub(Raison, "<CMD>", Room.Id);
        }
    }
}
