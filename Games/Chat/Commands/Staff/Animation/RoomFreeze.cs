namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomFreeze : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
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
