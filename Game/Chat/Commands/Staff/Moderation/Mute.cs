using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Game.Clients;
using Butterfly.Game.Users;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Mute : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (TargetUser.GetUser().Rank >= Session.GetUser().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
            }
            else
            {
                User user = TargetUser.GetUser();

                user.SpamProtectionTime = 300;
                user.SpamEnable = true;
                TargetUser.SendPacket(new FloodControlComposer(user.SpamProtectionTime));
            }
        }
    }
}
