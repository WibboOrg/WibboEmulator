using System;

namespace Butterfly.HabboHotel.Rooms
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
