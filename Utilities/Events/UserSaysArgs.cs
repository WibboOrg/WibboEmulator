namespace WibboEmulator.Utilities.Events;
using WibboEmulator.Games.Rooms;

public class UserSaysArgs : EventArgs
{
    public RoomUser User { get; private set; }
    public string Message { get; private set; }

    public UserSaysArgs(RoomUser user, string message)
    {
        this.User = user;
        this.Message = message;
    }
}
