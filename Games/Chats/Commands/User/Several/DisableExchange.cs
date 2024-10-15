namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableExchange : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (Session.User.AcceptTrading)
        {
            Session.User.AcceptTrading = false;
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.troc.true", Session.Language));
        }
        else
        {
            Session.User.AcceptTrading = true;
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.troc.false", Session.Language));
        }
    }
}
