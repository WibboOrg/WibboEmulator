namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisabledAutoGame : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (!WibboEnvironment.GetGame().GetAnimationManager().ToggleForceDisabled())
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.false", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.true", session.Langue));
        }
        return;
    }
}