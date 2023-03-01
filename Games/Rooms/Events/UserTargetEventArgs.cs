namespace WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Rooms;

public class UserTargetEventArgs : EventArgs
{
    public RoomUser UserTarget { get; private set; }

    public UserTargetEventArgs(RoomUser user) => this.UserTarget = user;
}
