namespace WibboEmulator.Utilities.Events;
using WibboEmulator.Games.Rooms;

public class UserWalksOnArgs : EventArgs
{
    public RoomUser User { get; private set; }

    public UserWalksOnArgs(RoomUser user) => this.User = user;
}
