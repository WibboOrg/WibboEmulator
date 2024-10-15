namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomFreeze : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.FreezeRoom = !room.FreezeRoom;

        if (room.FreezeRoom)
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.roomfreeze.true", session.Language));
        }
        else
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.roomfreeze.false", session.Language));
        }
    }
}
