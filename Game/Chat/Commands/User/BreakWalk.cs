using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class BreakWalk : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            UserRoom.breakwalk = !UserRoom.breakwalk;
            if (UserRoom.breakwalk)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.breakwalk.true", Session.Langue));
            }
            else
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.breakwalk.false", Session.Langue));
            }
        }
    }
}
