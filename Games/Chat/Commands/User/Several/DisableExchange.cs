namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableExchange : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
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
