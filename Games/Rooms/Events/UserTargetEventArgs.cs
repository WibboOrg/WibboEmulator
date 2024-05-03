namespace WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Rooms;

public class UserTargetEventArgs(RoomUser user) : EventArgs
{
    public RoomUser UserTarget { get; private set; } = user;
}
