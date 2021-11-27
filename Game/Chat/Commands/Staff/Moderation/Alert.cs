using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Alert : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
            {
                return;
            }

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else
            {
                string message = CommandManager.MergeParams(Params, 2);
                if (Session.Antipub(message, "<CMD>"))
                {
                    return;
                }

                TargetUser.SendNotification(message);
            }
        }
    }
}
