namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class FastWalk : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        userRoom.WalkSpeed = !userRoom.WalkSpeed;

        if (userRoom.WalkSpeed)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.walkppeed.true", Session.Language));
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.walkppeed.false", Session.Language));
        }
    }
}
