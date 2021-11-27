using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Disconnect : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (TargetUser.GetHabbo().Rank >= Session.GetHabbo().Rank)
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
