namespace WibboEmulator.Games.Chats.Commands.User.Room;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class OldFoot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.OldFoot = !room.OldFoot;

        if (room.OldFoot)
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.oldfoot.true", session.Language));
        }
        else
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.oldfoot.false", session.Language));
        }
    }
}
