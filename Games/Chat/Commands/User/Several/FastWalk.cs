namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class FastWalk : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        UserRoom.WalkSpeed = !UserRoom.WalkSpeed;

        if (UserRoom.WalkSpeed)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.false", session.Langue));
        }
    }
}
