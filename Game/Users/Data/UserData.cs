using Butterfly.Game.Achievement;
using Butterfly.Game.Users.Badges;
using Butterfly.Game.Users.Messenger;
using System.Collections.Generic;

namespace Butterfly.Game.Users.Data
{
    public class UserData
    {
        public int Id { get; private set; }
        public User User { get; private set; }

        public UserData(int id, User user)
        {
            this.Id = id;
            this.User = user;
        }
    }
}
