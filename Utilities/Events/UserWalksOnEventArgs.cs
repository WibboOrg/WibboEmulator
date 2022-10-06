namespace WibboEmulator.Utilities.Events;
using WibboEmulator.Games.Rooms;

public class UserWalksOnEventArgs : EventArgs
{
    public RoomUser User { get; private set; }

    public UserWalksOnEventArgs(RoomUser user) => this.User = user;
}
