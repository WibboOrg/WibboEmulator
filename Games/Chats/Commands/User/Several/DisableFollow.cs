namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableFollow : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (Session.User.HideInRoom)
        {
            Session.User.HideInRoom = false;
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.followme.true", Session.Language));
        }
        else
        {
            Session.User.HideInRoom = true;
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.followme.false", Session.Language));
        }
    }
}
