using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class DisabledAutoGame : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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