using Wibbo.Game.Clients;
using Wibbo.Game.Rooms.Games;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class FaceWalk : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            RoomUser roomUserByUserId = UserRoom;
            roomUserByUserId.FacewalkEnabled = !roomUserByUserId.FacewalkEnabled;
            if (roomUserByUserId.FacewalkEnabled)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.true", Session.Langue));
            }
            else
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.false", Session.Langue));
            }
        }
    }
}
