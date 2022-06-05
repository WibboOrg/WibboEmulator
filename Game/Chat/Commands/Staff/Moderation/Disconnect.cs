using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class Disconnect : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (TargetUser.GetUser().Rank >= Session.GetUser().Rank)
            {
                //Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                Session.Disconnect();
            }
            else
            {
                TargetUser.Disconnect();
            }

        }
    }
}
