namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class MachineBan : IChatCommand
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
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
        else if (string.IsNullOrEmpty(clientByUsername.MachineId))
        {
            return;
        }
        else if (clientByUsername.User.Rank >= session.User.Rank)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
            WibboEnvironment.GetGame().GetGameClientManager().BanUser(session, "Robot", 788922000, "Votre compte a été banni par sécurité", false, false);
        }
        else
        {
            var raison = "";
            if (parameters.Length > 2)
            {
                raison = CommandManager.MergeParams(parameters, 2);
            }

            WibboEnvironment.GetGame().GetGameClientManager().BanUser(clientByUsername, session.User.Username, 788922000, raison, true, true);
            session.SendWhisper("Tu viens de bannir " + clientByUsername.User.Username + " pour la raison : " + raison + " !");
            _ = session.User.Antipub(raison, "<CMD>", room.Id);
            return;
        }
    }
}
