namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomFreeze : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.FreezeRoom = !room.FreezeRoom;

        if (room.FreezeRoom)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.roomfreeze.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.roomfreeze.false", session.Langue));
        }
    }
}
