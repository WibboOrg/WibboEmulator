using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Users;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Mute : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (TargetUser.GetUser().Rank >= Session.GetUser().Rank)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
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
