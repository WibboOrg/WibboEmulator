using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class GameTime : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (WibboEnvironment.GetGame().GetAnimationManager().IsActivate())
            {
                string Time = WibboEnvironment.GetGame().GetAnimationManager().GetTime();
                Session.SendWhisper("Prochaine animation de Jack & Daisy dans " + Time);
            }
            else
            {
                Session.SendWhisper("Les animations de Jack & Daisy sont désactivées");
            }
        }
    }
}
