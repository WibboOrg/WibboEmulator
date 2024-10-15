namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.Animations;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisabledAutoGame : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (!AnimationManager.ToggleForceDisabled)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.autogame.false", Session.Language));
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.autogame.true", Session.Language));
        }
        return;
    }
}
