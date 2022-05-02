using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
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
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.true", Session.Langue));
            }
            else
            {
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.false", Session.Langue));
            }
        }
    }
}
