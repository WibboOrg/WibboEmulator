using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class GameTime : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (ButterflyEnvironment.GetGame().GetAnimationManager().IsActivate())
            {
                string Time = ButterflyEnvironment.GetGame().GetAnimationManager().GetTime();
                Session.SendWhisper("Prochaine animation de Jack & Daisy dans " + Time);
            }
            else
            {
                Session.SendWhisper("Les animations de Jack & Daisy sont désactivées");
            }
        }
    }
}
