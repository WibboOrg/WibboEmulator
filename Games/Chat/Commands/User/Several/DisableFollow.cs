namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableFollow : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.User.HideInRoom)
        {
            session.User.HideInRoom = false;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.followme.true", session.Langue));
        }
        else
        {
            session.User.HideInRoom = true;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.followme.false", session.Langue));
        }
    }
}
