using Butterfly.Game.Achievement;
using Butterfly.Game.Users.Badges;
using Butterfly.Game.Users.Messenger;
using System.Collections.Generic;

namespace Butterfly.Game.Users.Data
{
    public class UserData
    {
        public int Id { get; private set; }
        public Dictionary<string, UserAchievement> Achievements { get; private set; }
        public List<int> FavouritedRooms { get; private set; }
        public List<Badge> Badges { get; private set; }
        public Dictionary<int, MessengerBuddy> Friends { get; private set; }
        public Dictionary<int, MessengerRequest> Requests { get; private set; }
        public Dictionary<int, int> Quests { get; private set; }
        public List<int> MyGroups { get; private set; }
        public User User { get; private set; }
        public Dictionary<int, Relationship> Relationships { get; private set; }
        public List<int> RoomRightsList { get; private set; }

        public UserData(int id, Dictionary<string, UserAchievement> achievements, List<int> favouritedRooms, List<Badge> badges, Dictionary<int, MessengerBuddy> friends, Dictionary<int, MessengerRequest> requests, Dictionary<int, int> quests, List<int> MyGroups, User user, Dictionary<int, Relationship> relationships, List<int> RoomRightsList)
        {
            this.Id = id;
            this.Achievements = achievements;
            this.FavouritedRooms = favouritedRooms;
            this.Badges = badges;
            this.Friends = friends;
            this.Requests = requests;
            this.Quests = quests;
            this.User = user;
            this.MyGroups = MyGroups;
            this.Relationships = relationships;
            this.RoomRightsList = RoomRightsList;
        }
    }
}
