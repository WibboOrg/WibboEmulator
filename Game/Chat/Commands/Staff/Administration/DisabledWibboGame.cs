using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Game.Clients;
using System.Linq;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class DisabledWibboGame : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!ButterflyEnvironment.GetGame().GetAnimationManager().ToggleForceDisabled())
            {
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.false", Session.Langue));
            }
            else
            {
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.true", Session.Langue));
            }
            return;
        }
    }
}