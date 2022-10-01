using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class VipProtect : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().PremiumProtect = !Session.GetUser().PremiumProtect;

            if (Session.GetUser().PremiumProtect)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.premium.true", Session.Langue));
            }
            else
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.premium.false", Session.Langue));
            }
        }
    }
}
