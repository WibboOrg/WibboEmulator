using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Override : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.AllowOverride)
            {
                UserRoom.AllowOverride = false;
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("override.disabled", Session.Langue));
            }
            else
            {
                UserRoom.AllowOverride = true;
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("override.enabled", Session.Langue));
            }
        }
    }
}
