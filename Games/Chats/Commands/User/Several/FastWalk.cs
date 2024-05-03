namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class FastWalk : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        userRoom.WalkSpeed = !userRoom.WalkSpeed;

        if (userRoom.WalkSpeed)
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.walkppeed.true", session.Language));
        }
        else
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.walkppeed.false", session.Language));
        }
    }
}
