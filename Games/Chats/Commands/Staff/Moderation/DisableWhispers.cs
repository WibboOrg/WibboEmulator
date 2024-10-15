namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableWhispers : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.User.ViewMurmur)
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.murmur.true", session.Language));
        }
        else
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.murmur.false", session.Language));
        }

        session.User.ViewMurmur = !session.User.ViewMurmur;
    }
}
