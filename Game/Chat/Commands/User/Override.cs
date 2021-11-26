using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Override : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.AllowOverride)
            {
                UserRoom.AllowOverride = false;
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("override.disabled", Session.Langue));
            }
            else
            {
                UserRoom.AllowOverride = true;
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("override.enabled", Session.Langue));
            }
        }
    }
}