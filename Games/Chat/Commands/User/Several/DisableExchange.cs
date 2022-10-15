namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableExchange : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.User.AcceptTrading)
        {
            session.User.AcceptTrading = false;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.troc.true", session.Langue));
        }
        else
        {
            session.User.AcceptTrading = true;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.troc.false", session.Langue));
        }
    }
}
