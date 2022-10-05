namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableExchange : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (session.GetUser().AcceptTrading)
        {
            session.GetUser().AcceptTrading = false;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.troc.true", session.Langue));
        }
        else
        {
            session.GetUser().AcceptTrading = true;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.troc.false", session.Langue));
        }
    }
}
