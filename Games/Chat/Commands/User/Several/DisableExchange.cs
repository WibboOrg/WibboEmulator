using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class DisableExchange : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetUser().AcceptTrading)
            {
                Session.GetUser().AcceptTrading = false;
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.troc.true", Session.Langue));
            }
            else
            {
                Session.GetUser().AcceptTrading = true;
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.troc.false", Session.Langue));
            }
        }
    }
}
