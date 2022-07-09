using Wibbo.Game.Clients;
using Wibbo.Game.Rooms.Games;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class MoonWalk : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            Room currentRoom = Session.GetUser().CurrentRoom;
            if (currentRoom == null || UserRoom.InGame)
            {
                return;
            }

            RoomUser roomUserByUserId = UserRoom;
            roomUserByUserId.MoonwalkEnabled = !roomUserByUserId.MoonwalkEnabled;
            if (roomUserByUserId.MoonwalkEnabled)
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
