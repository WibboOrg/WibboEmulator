namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class FaceWalk : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        UserRoom.FacewalkEnabled = !UserRoom.FacewalkEnabled;
        if (UserRoom.FacewalkEnabled)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.false", session.Langue));
        }
    }
}
