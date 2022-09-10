using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms.Games;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class MoonWalk : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            if (UserRoom.InGame)
            {
                return;
            }

            UserRoom.MoonwalkEnabled = !UserRoom.MoonwalkEnabled;
            if (UserRoom.MoonwalkEnabled)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.true", Session.Langue));
            }
            else
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.false", Session.Langue));
            }
        }
    }
}
