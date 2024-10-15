namespace WibboEmulator.Games.Chats.Commands.User.Build;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class UseStop : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        Session.User.ForceUse = -1;

        Session.SendWhisper(LanguageManager.TryGetValue("cmd.use.disabled", Session.Language));
    }
}
