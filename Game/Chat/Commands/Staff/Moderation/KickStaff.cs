using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class KickStaff : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetUser().Rank <= TargetUser.GetUser().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
            }
            else if (TargetUser.GetUser().CurrentRoomId < 1U)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("kick.error", Session.Langue));
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

                    TargetUser.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("kick.withmessage", TargetUser.Langue) + message);
                }
                else
                {
                    TargetUser.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("kick.nomessage", TargetUser.Langue));
                }
            }

        }
    }
}
