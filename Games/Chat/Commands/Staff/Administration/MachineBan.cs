namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MachineBan : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (clientByUsername == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
        else if (string.IsNullOrEmpty(clientByUsername.MachineId))
        {
            return;
        }
        else if (clientByUsername.GetUser().Rank >= session.GetUser().Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
            WibboEnvironment.GetGame().GetGameClientManager().BanUser(session, "Robot", 788922000, "Votre compte a été banni par sécurité", false, false);
        }
        else
        {
            var raison = "";
            if (parameters.Length > 2)
            {
                raison = CommandManager.MergeParams(parameters, 2);
            }

            WibboEnvironment.GetGame().GetGameClientManager().BanUser(clientByUsername, session.GetUser().Username, 788922000, raison, true, true);
            session.SendWhisper("Tu viens de bannir " + clientByUsername.GetUser().Username + " pour la raison : " + raison + " !");
            _ = session.Antipub(raison, "<CMD>");
            return;
        }
    }
}
