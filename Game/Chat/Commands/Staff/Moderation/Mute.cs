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
            if (TargetUser == null || TargetUser.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (TargetUser.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
            }
            else
            {
                User habbo = TargetUser.GetHabbo();

                habbo.SpamProtectionTime = 300;
                habbo.SpamEnable = true;
                TargetUser.SendPacket(new FloodControlComposer(habbo.SpamProtectionTime));
            }
        }
    }
}
