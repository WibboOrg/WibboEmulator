namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableExchange : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.User.AcceptTrading)
        {
            session.User.AcceptTrading = false;
            session.SendWhisper(LanguageManager.TryGetValue("cmd.troc.true", session.Language));
        }
        else
        {
            session.User.AcceptTrading = true;
            session.SendWhisper(LanguageManager.TryGetValue("cmd.troc.false", session.Language));
        }
    }
}
