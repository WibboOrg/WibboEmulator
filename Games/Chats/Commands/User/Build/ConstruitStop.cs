namespace WibboEmulator.Games.Chats.Commands.User.Build;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ConstruitStop : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        userRoom.BuildToolEnable = false;

        Session.SendWhisper(LanguageManager.TryGetValue("cmd.construit.disabled", Session.Language));
    }
}
