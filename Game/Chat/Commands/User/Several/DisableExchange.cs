using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class DisableExchange : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetUser().AcceptTrading)
            {
                Session.GetUser().AcceptTrading = false;
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.troc.true", Session.Langue));
            }
            else
            {
                Session.GetUser().AcceptTrading = true;
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.troc.false", Session.Langue));
            }

        }
    }
}
