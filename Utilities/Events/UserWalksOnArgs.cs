using Wibbo.Game.Rooms;

namespace Wibbo.Utilities.Events
{
    public class UserWalksOnArgs : EventArgs
    {
        public readonly RoomUser User;

        public UserWalksOnArgs(RoomUser user)
        {
            this.User = user;
        }
    }
}
