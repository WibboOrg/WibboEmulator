namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MachineBan : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
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
            var Raison = "";
            if (parameters.Length > 2)
            {
                Raison = CommandManager.MergeParams(parameters, 2);
            }

            WibboEnvironment.GetGame().GetGameClientManager().BanUser(clientByUsername, session.GetUser().Username, 788922000, Raison, true, true);
            session.SendWhisper("Tu viens de bannir " + clientByUsername.GetUser().Username + " pour la raison : " + Raison + " !");
            session.Antipub(Raison, "<CMD>");
            return;
        }
    }
}
