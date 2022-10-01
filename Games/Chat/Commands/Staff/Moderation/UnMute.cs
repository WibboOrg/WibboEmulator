using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Users;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class UnMute : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else
            {
                User user = TargetUser.GetUser();

                user.SpamProtectionTime = 10;
                user.SpamEnable = true;
            }
        }
    }
}
