namespace WibboEmulator.Games.Chat.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Use : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var Count = parameters[1];
        if (!int.TryParse(Count, out var UseCount))
        {
            return;
        }

        if (UseCount is < 0 or > 100)
        {
            return;
        }

        session.GetUser().ForceUse = UseCount;

        session.SendWhisper("Use: " + UseCount);
    }
}
