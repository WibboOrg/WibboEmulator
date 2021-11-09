using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class MoonWalk : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
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
