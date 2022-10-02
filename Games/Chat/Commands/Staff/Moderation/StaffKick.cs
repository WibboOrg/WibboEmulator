using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class StaffKick : IChatCommand
    {
        public void Execute(GameClient session, Room room, RoomUser user, string[] parameters)
        {
            if (parameters.Length < 2)
                return;

            GameClient TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            }
            else if (session.GetUser().Rank <= TargetUser.GetUser().Rank)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
            }
            else if (TargetUser.GetUser().CurrentRoomId < 1U)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.error", session.Langue));
            }
            else
            {
                room.GetRoomUserManager().RemoveUserFromRoom(TargetUser, true, false);

                if (parameters.Length > 2)
                {
                    string message = CommandManager.MergeParams(parameters, 2);
                    if (session.Antipub(message, "<CMD>", room.Id))
                    {
                        return;
                    }

                    TargetUser.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.withmessage", TargetUser.Langue) + message);
                }
                else
                {
                    TargetUser.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.nomessage", TargetUser.Langue));
                }
            }
        }
    }
}
