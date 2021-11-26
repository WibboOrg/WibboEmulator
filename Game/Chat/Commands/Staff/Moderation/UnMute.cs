using Butterfly.Game.Clients;
using Butterfly.Game.Users;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class UnMute : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else
            {
                User habboByUsername = clientByUsername.GetHabbo();

                habboByUsername.spamProtectionTime = 10;
                habboByUsername.spamEnable = true;
            }

        }
    }
}
