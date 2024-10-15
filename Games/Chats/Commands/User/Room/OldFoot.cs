namespace WibboEmulator.Games.Chats.Commands.User.Room;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class OldFoot : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.OldFoot = !room.OldFoot;

        if (room.OldFoot)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.oldfoot.true", Session.Language));
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.oldfoot.false", Session.Language));
        }
    }
}
