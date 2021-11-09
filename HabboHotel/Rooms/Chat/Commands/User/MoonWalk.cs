using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class MoonWalk : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null || UserRoom.InGame)
            {
                return;
            }

            RoomUser roomUserByHabbo = UserRoom;
            roomUserByHabbo.MoonwalkEnabled = !roomUserByHabbo.MoonwalkEnabled;
            if (roomUserByHabbo.MoonwalkEnabled)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.true", Session.Langue));
            }
            else
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.false", Session.Langue));
            }
        }
    }
}
