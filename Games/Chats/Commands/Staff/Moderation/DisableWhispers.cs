namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableWhispers : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.User.ViewMurmur)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.murmur.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.murmur.false", session.Langue));
        }

        session.User.ViewMurmur = !session.User.ViewMurmur;
    }
}
