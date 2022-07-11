using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class DisabledWibboGame : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!WibboEnvironment.GetGame().GetAnimationManager().ToggleForceDisabled())
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.false", Session.Langue));
            }
            else
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.true", Session.Langue));
            }
            return;
        }
    }
}