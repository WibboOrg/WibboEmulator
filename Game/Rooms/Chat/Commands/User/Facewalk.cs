using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Facewalk : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            RoomUser roomUserByHabbo = UserRoom;
            roomUserByHabbo.FacewalkEnabled = !roomUserByHabbo.FacewalkEnabled;
            if (roomUserByHabbo.FacewalkEnabled)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.true", Session.Langue));
            }
            else
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.false", Session.Langue));
            }
        }
    }
}
