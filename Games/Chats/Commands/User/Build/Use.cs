namespace WibboEmulator.Games.Chats.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Use : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var count = parameters[1];
        if (!int.TryParse(count, out var useCount))
        {
            return;
        }

        if (useCount is < 0 or > 100)
        {
            return;
        }

        session.User.ForceUse = useCount;

        session.SendWhisper("Use: " + useCount);
    }
}
