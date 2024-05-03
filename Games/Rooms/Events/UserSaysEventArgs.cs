namespace WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Rooms;

public class UserSaysEventArgs(RoomUser user, string message) : EventArgs
{
    public RoomUser User { get; private set; } = user;
    public string Message { get; private set; } = message;
    public bool Result { get; set; }
}
