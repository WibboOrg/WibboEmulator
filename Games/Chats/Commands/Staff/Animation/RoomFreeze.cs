namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomFreeze : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.FreezeRoom = !room.FreezeRoom;

        if (room.FreezeRoom)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.roomfreeze.true", Session.Language));
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.roomfreeze.false", Session.Language));
        }
    }
}
