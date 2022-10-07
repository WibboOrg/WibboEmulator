namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class FastWalk : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        userRoom.WalkSpeed = !userRoom.WalkSpeed;

        if (userRoom.WalkSpeed)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.false", session.Langue));
        }
    }
}
