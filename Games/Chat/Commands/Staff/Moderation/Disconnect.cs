using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Disconnect : IChatCommand
    {
        public void Execute(Client session, Room room, RoomUser roomUser, string[] parameters)
        {
            if (parameters.Length < 2)
                return;

            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(parameters[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            }
            else if (TargetUser.GetUser().Rank >= session.GetUser().Rank)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
            }
            else
            {
                TargetUser.Disconnect();
            }
        }
    }
}
