using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class StaffKick : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetUser().Rank <= TargetUser.GetUser().Rank)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
            }
            else if (TargetUser.GetUser().CurrentRoomId < 1U)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("kick.error", Session.Langue));
            }
            else
            {
                Room.GetRoomUserManager().RemoveUserFromRoom(TargetUser, true, false);

                if (Params.Length > 2)
                {
                    string message = CommandManager.MergeParams(Params, 2);
                    if (Session.Antipub(message, "<CMD>", Room.Id))
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
