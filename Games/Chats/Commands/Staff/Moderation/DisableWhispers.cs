namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableWhispers : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (Session.User.ViewMurmur)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.murmur.true", Session.Language));
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.murmur.false", Session.Language));
        }

        Session.User.ViewMurmur = !Session.User.ViewMurmur;
    }
}
