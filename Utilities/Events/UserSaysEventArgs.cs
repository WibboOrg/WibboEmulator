namespace WibboEmulator.Utilities.Events;
using WibboEmulator.Games.Rooms;

public class UserSaysEventArgs : EventArgs
{
    public RoomUser User { get; private set; }
    public string Message { get; private set; }
    public bool Result { get; set; }

    public UserSaysEventArgs(RoomUser user, string message)
    {
        this.User = user;
        this.Message = message;
    }
}
