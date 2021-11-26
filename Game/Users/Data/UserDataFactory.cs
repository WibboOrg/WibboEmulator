using Butterfly.Core;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Achievement;
using Butterfly.Game.Users.Badges;
using Butterfly.Game.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Users.Data
{
    public class UserDataFactory
    {
        public static UserData GetUserData(string sessionTicket, string ip, string machineid)
        {
            try
            {
                int userId;
                DataRow dUserInfo;
                DataRow row2;
                DataTable Achievement;
                DataTable Favorites;
                DataTable RoomRights;
                DataTable Badges;
                DataTable FrienShips;
                DataTable Requests;
                DataTable Quests;
                DataTable GroupMemberships;
                int ignoreAllExpire = 0;
                bool ChangeName = false;

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dUserInfo = UserDao.GetOneByTicket(dbClient, sessionTicket);
                    if (dUserInfo == null)
                    {
                        return null;
                    }

                    bool IsBanned = BanDao.IsBanned(dbClient, dUserInfo["username"].ToString(), ip, dUserInfo["ip_last"].ToString(), machineid);
                    if (IsBanned)
                    {
                        return null;
                    }

                    int IgnoreAll = BanDao.GetOneIgnoreAll(dbClient, dUserInfo["username"].ToString());
                    if (IgnoreAll > 0)
                    {
                        ignoreAllExpire = IgnoreAll;
                    }

                    userId = Convert.ToInt32(dUserInfo["id"]);
                    string username = (string)dUserInfo["username"];
                    if (ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId) != null)
                    {
                        ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId).Disconnect();
                        return null;
                    }

                    string lastDailyCredits = (string)dUserInfo["lastdailycredits"];
                    string lastDaily = DateTime.Today.ToString("MM/dd");
                    if (lastDailyCredits != lastDaily)
                    {
                        UserDao.UpdateLastDailyCredits(dbClient, userId, lastDaily);
                        dUserInfo["credits"] = (Convert.ToInt32(dUserInfo["credits"]) + 3000);

                        if (Convert.ToInt32(dUserInfo["rank"]) <= 1)
                        {
                            UserStatsDao.UpdateRespectPoint(dbClient, userId, 5);
                        }
                        else
                        {
                            UserStatsDao.UpdateRespectPoint(dbClient, userId, 20);
                        }

                        ChangeName = true;
                    }

                    if (!sessionTicket.StartsWith("monticket"))
                    {
                        UserDao.UpdateOnline(dbClient, userId);
                    }

                    row2 = UserStatsDao.GetOne(dbClient, userId);

                    if (row2 == null)
                    {
                        UserStatsDao.Insert(dbClient, userId);
                        row2 = UserStatsDao.GetOne(dbClient, userId);
                    }

                    Achievement = UserAchievementDao.GetAll(dbClient, userId);

                    Favorites = UserFavoriteDao.GetAll(dbClient, userId);

                    RoomRights = RoomRightDao.GetAllByUserId(dbClient, userId);

                    Badges = UserBadgeDao.GetAll(dbClient, userId);

                    FrienShips = UserDao.GetAllFriendShips(dbClient, userId);

                    Requests = UserDao.GetAllFriendRequests(dbClient, userId);

                    Quests = UserQuestDao.GetAll(dbClient, userId);

                    GroupMemberships = GuildMembershipDao.GetOneByUserId(dbClient, userId);
                }

                Dictionary<string, UserAchievement> achievements = new Dictionary<string, UserAchievement>();
                foreach (DataRow dataRow in Achievement.Rows)
                {
                    string str = (string)dataRow["group"];
                    int level = Convert.ToInt32(dataRow["level"]);
                    int progress = Convert.ToInt32(dataRow["progress"]);
                    UserAchievement userAchievement = new UserAchievement(str, level, progress);
                    achievements.Add(str, userAchievement);
                }

                if (!achievements.ContainsKey("ACH_CameraPhotoCount"))
                {
                    UserAchievement userAchievement = new UserAchievement("ACH_CameraPhotoCount", 10, 0);
                    achievements.Add("ACH_CameraPhotoCount", userAchievement);
                }

                List<int> roomRightsList = new List<int>();
                foreach (DataRow dataRow in RoomRights.Rows)
                {
                    int num3 = Convert.ToInt32(dataRow["room_id"]);
                    roomRightsList.Add(num3);
                }

                List<int> favouritedRooms = new List<int>();
                foreach (DataRow dataRow in Favorites.Rows)
                {
                    int num3 = Convert.ToInt32(dataRow["room_id"]);
                    favouritedRooms.Add(num3);
                }

                List<Badge> badges = new List<Badge>();
                foreach (DataRow dataRow in Badges.Rows)
                {
                    string Code = (string)dataRow["badge_id"];
                    int Slot = Convert.ToInt32(dataRow["badge_slot"]);
                    badges.Add(new Badge(Code, Slot));
                }

                Dictionary<int, Relationship> relationShips = new Dictionary<int, Relationship>();
                Dictionary<int, MessengerBuddy> friends = new Dictionary<int, MessengerBuddy>();
                foreach (DataRow dataRow in FrienShips.Rows)
                {
                    int num3 = Convert.ToInt32(dataRow["id"]);
                    string pUsername = (string)dataRow["username"];
                    int Relation = Convert.ToInt32(dataRow["relation"]);
                    if (num3 != userId)
                    {
                        if (!friends.ContainsKey(num3))
                        {
                            friends.Add(num3, new MessengerBuddy(num3, pUsername, "", Relation));
                            if (Relation != 0)
                            {
                                relationShips.Add(num3, new Relationship(num3, Relation));
                            }
                        }
                    }
                }

                Dictionary<int, MessengerRequest> requests = new Dictionary<int, MessengerRequest>();
                foreach (DataRow dataRow in Requests.Rows)
                {
                    int num3 = Convert.ToInt32(dataRow["from_id"]);
                    int num4 = Convert.ToInt32(dataRow["to_id"]);
                    string pUsername = (string)dataRow["username"];
                    if (num3 != userId)
                    {
                        if (!requests.ContainsKey(num3))
                        {
                            requests.Add(num3, new MessengerRequest(userId, num3, pUsername));
                        }
                    }
                    else if (!requests.ContainsKey(num4))
                    {
                        requests.Add(num4, new MessengerRequest(userId, num4, pUsername));
                    }
                }

                Dictionary<int, int> quests = new Dictionary<int, int>();
                foreach (DataRow dataRow in Quests.Rows)
                {
                    int key = Convert.ToInt32(dataRow["quest_id"]);
                    int num3 = Convert.ToInt32(dataRow["progress"]);
                    quests.Add(key, num3);
                }

                List<int> myGroups = new List<int>();
                foreach (DataRow dRow in GroupMemberships.Rows)
                {
                    myGroups.Add(Convert.ToInt32(dRow["group_id"]));
                }
                User user = GenerateHabbo(dUserInfo, row2, ChangeName, ignoreAllExpire);

                return new UserData(userId, achievements, favouritedRooms, badges, friends, requests, quests, myGroups, user, relationShips, roomRightsList);
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "UserDataFactory.GetUserData");            
                return null;
            }
        }

        public static UserData GetUserData(int userId)
        {
            DataRow row;
            DataRow row2;
            DataTable FrienShips;

            if (ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId) != null)
            {
                return null;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                row = UserDao.GetOne(dbClient, userId);
                if (row == null)
                {
                    return null;
                }

                row2 = UserStatsDao.GetOne(dbClient, userId);

                if (row2 == null)
                {
                    UserStatsDao.Insert(dbClient, userId);
                    row2 = UserStatsDao.GetOne(dbClient, userId);
                }

                FrienShips = UserDao.GetAllFriendRelation(dbClient, userId);
            }

            Dictionary<int, MessengerBuddy> friends = new Dictionary<int, MessengerBuddy>();
            Dictionary<int, Relationship> Relationships = new Dictionary<int, Relationship>();
            foreach (DataRow dataRow in FrienShips.Rows)
            {
                int num3 = Convert.ToInt32(dataRow["id"]);
                int Relation = Convert.ToInt32(dataRow["relation"]);
                if (num3 != userId)
                {
                    if (!friends.ContainsKey(num3))
                    {
                        if (Relation != 0)
                        {
                            Relationships.Add(num3, new Relationship(num3, Relation));
                        }
                    }
                }
            }

            Dictionary<string, UserAchievement> achievements = new Dictionary<string, UserAchievement>();
            List<int> favouritedRooms = new List<int>();
            List<int> RoomRight = new List<int>();
            List<Badge> badges = new List<Badge>();
            Dictionary<int, MessengerRequest> requests = new Dictionary<int, MessengerRequest>();
            Dictionary<int, int> quests = new Dictionary<int, int>();
            List<int> MyGroups = new List<int>();

            User user = GenerateHabbo(row, row2, false, 0);
            return new UserData(userId, achievements, favouritedRooms, badges, friends, requests, quests, MyGroups, user, Relationships, RoomRight);
        }

        public static User GenerateHabbo(DataRow dRow, DataRow dRow2, bool ChangeName, int ignoreAllExpire)
        {
            int Id = Convert.ToInt32(dRow["id"]);
            string Username = (string)dRow["username"];
            int Rank = Convert.ToInt32(dRow["rank"]);
            string Motto = (string)dRow["motto"];
            string Look = (string)dRow["look"];
            string Gender = (string)dRow["gender"];
            int LastOnline = Convert.ToInt32(dRow["last_online"]);
            int Credits = Convert.ToInt32(dRow["credits"]);
            int Diamonds = Convert.ToInt32(dRow["vip_points"]);
            int ActivityPoints = Convert.ToInt32(dRow["activity_points"]);
            int HomeRoom = Convert.ToInt32(dRow["home_room"]);
            int Respect = Convert.ToInt32(dRow2["respect"]);
            int DailyRespectPoints = Convert.ToInt32(dRow2["daily_respect_points"]);
            int DailyPetRespectPoints = Convert.ToInt32(dRow2["daily_pet_respect_points"]);
            bool HasFriendRequestsDisabled = ButterflyEnvironment.EnumToBool(dRow["block_newfriends"].ToString());
            int currentQuestID = Convert.ToInt32(dRow2["quest_id"]);
            int achievementPoints = Convert.ToInt32(dRow2["achievement_score"]);
            int FavoriteGroup = Convert.ToInt32(dRow2["group_id"]);
            int accountCreated = Convert.ToInt32(dRow["account_created"]);
            bool AcceptTrading = ButterflyEnvironment.EnumToBool(dRow["accept_trading"].ToString());
            string Ip = dRow["ip_last"].ToString();
            bool HideInroom = ButterflyEnvironment.EnumToBool(dRow["hide_inroom"].ToString());
            bool HideOnline = ButterflyEnvironment.EnumToBool(dRow["hide_online"].ToString());
            int MazoHighScore = Convert.ToInt32(dRow["mazoscore"]);
            int Mazo = Convert.ToInt32(dRow["mazo"]);
            string clientVolume = (string)dRow["volume"];
            bool NuxEnable = ButterflyEnvironment.EnumToBool(dRow["nux_enable"].ToString());
            string MachineId = (string)dRow["machine_id"];
            Language Langue = LanguageManager.ParseLanguage((string)dRow["langue"]);

            return new User(Id, Username, Rank, Motto, Look, Gender, Credits, Diamonds, ActivityPoints, HomeRoom, Respect, DailyRespectPoints, DailyPetRespectPoints, HasFriendRequestsDisabled, currentQuestID, achievementPoints, LastOnline, FavoriteGroup, accountCreated, AcceptTrading, Ip, HideInroom, HideOnline, MazoHighScore, Mazo, clientVolume, NuxEnable, MachineId, ChangeName, Langue, ignoreAllExpire);
        }
    }
}
