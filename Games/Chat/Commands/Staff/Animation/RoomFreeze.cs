namespace WibboEmulator.Games.Chat.Commands.Staff.Animation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomFreeze : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        Room.FreezeRoom = !Room.FreezeRoom;

        if (Room.FreezeRoom)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.roomfreeze.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.roomfreeze.false", session.Langue));
        }
    }
}
