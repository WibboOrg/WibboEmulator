using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms.Games;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class FaceWalk : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            UserRoom.FacewalkEnabled = !UserRoom.FacewalkEnabled;
            if (UserRoom.FacewalkEnabled)
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
