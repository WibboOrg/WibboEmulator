using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;
using System.Data;

namespace WibboEmulator.Games.Navigator
{
    internal static class NavigatorHandler
    {
        public static void Search(ServerPacket Message, SearchResultList SearchResult, string SearchData, GameClient Session, int FetchLimit)
        {
            //Switching by categorys.
            switch (SearchResult.CategoryType)
            {
                default:
                    Message.WriteInteger(0);
                    break;

                case NavigatorCategoryType.QUERY:
                    {
                        if (SearchData.ToLower().StartsWith("owner:"))
                        {
                            if (SearchData.Length > 0)
                            {
                                DataTable GetRooms = null;
                                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    if (SearchData.ToLower().StartsWith("owner:"))
                                    {
                                        GetRooms = RoomDao.GetAllSearchByUsername(dbClient, SearchData.Remove(0, 6));
                                    }
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (GetRooms != null)
                                {
                                    foreach (DataRow Row in GetRooms.Rows)
                                    {
                                        RoomData RoomData = WibboEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
                                        if (RoomData != null && !Results.Contains(RoomData))
                                        {
                                            Results.Add(RoomData);
                                        }
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data);
                                }
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("tag:"))
                        {
                            SearchData = SearchData.Remove(0, 4);
                            ICollection<RoomData> TagMatches = WibboEnvironment.GetGame().GetRoomManager().SearchTaggedRooms(SearchData);

                            Message.WriteInteger(TagMatches.Count);
                            foreach (RoomData Data in TagMatches.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data);
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("group:"))
                        {
                            SearchData = SearchData.Remove(0, 6);
                            ICollection<RoomData> GroupRooms = WibboEnvironment.GetGame().GetRoomManager().SearchGroupRooms(SearchData);

                            Message.WriteInteger(GroupRooms.Count);
                            foreach (RoomData Data in GroupRooms.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data);
                            }
                        }
                        else
                        {
                            if (SearchData.Length > 0)
                            {
                                DataTable Table = null;
                                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    Table = RoomDao.GetAllSearch(dbClient, SearchData);
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (Table != null)
                                {
                                    foreach (DataRow Row in Table.Rows)
                                    {
                                        if (Convert.ToString(Row["state"]) == "invisible")
                                        {
                                            continue;
                                        }

                                        RoomData RData = WibboEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
                                        if (RData != null && !Results.Contains(RData))
                                        {
                                            Results.Add(RData);
                                        }
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data);
                                }
                            }
                        }

                        break;
                    }

                case NavigatorCategoryType.FEATURED:
                case NavigatorCategoryType.FEATURED_GAME:
                case NavigatorCategoryType.FEATURED_NEW:
                case NavigatorCategoryType.FEATURED_HELP_SECURITY:
                case NavigatorCategoryType.FEATURED_RUN:
                case NavigatorCategoryType.FEATURED_CASINO:
                    List<RoomData> Rooms = new List<RoomData>();
                    ICollection<FeaturedRoom> Featured = WibboEnvironment.GetGame().GetNavigator().GetFeaturedRooms(Session.Langue);
                    foreach (FeaturedRoom FeaturedItem in Featured.ToList())
                    {
                        if (FeaturedItem == null)
                        {
                            continue;
                        }

                        if (FeaturedItem.CategoryType != SearchResult.CategoryType)
                        {
                            continue;
                        }

                        RoomData Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(FeaturedItem.RoomId);
                        if (Data == null)
                        {
                            continue;
                        }

                        if (!Rooms.Contains(Data))
                        {
                            Rooms.Add(Data);
                        }
                    }

                    Message.WriteInteger(Rooms.Count);
                    foreach (RoomData Data in Rooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;

                case NavigatorCategoryType.POPULAR:
                    {
                        List<RoomData> PopularRooms = new List<RoomData>();

                        PopularRooms.AddRange(WibboEnvironment.GetGame().GetRoomManager().GetPopularRooms(-1, 50, Session.Langue)); //FetchLimit

                        Message.WriteInteger(PopularRooms.Count);
                        foreach (RoomData Data in PopularRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data);
                        }
                        break;
                    }

                case NavigatorCategoryType.RECOMMENDED:
                    {
                        List<RoomData> RecommendedRooms = WibboEnvironment.GetGame().GetRoomManager().GetRecommendedRooms(FetchLimit);

                        Message.WriteInteger(RecommendedRooms.Count);
                        foreach (RoomData Data in RecommendedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data);
                        }
                        break;
                    }

                case NavigatorCategoryType.CATEGORY:
                    {
                        List<RoomData> GetRoomsByCategory = WibboEnvironment.GetGame().GetRoomManager().GetRoomsByCategory(SearchResult.Id, FetchLimit);

                        Message.WriteInteger(GetRoomsByCategory.Count);
                        foreach (RoomData Data in GetRoomsByCategory.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data);
                        }
                        break;
                    }

                case NavigatorCategoryType.MY_ROOMS:

                    List<RoomData> MyRooms = new List<RoomData>();

                    foreach (int RoomId in Session.GetUser().UsersRooms)
                    {
                        RoomData Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
                        if (Data == null)
                        {
                            continue;
                        }

                        if (!MyRooms.Contains(Data))
                        {
                            MyRooms.Add(Data);
                        }
                    }

                    Message.WriteInteger(MyRooms.Count);
                    foreach (RoomData Data in MyRooms.OrderBy(a => a.Name).ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;

                case NavigatorCategoryType.MY_FAVORITES:
                    List<RoomData> Favourites = new List<RoomData>();
                    foreach (int RoomId in Session.GetUser().FavoriteRooms)
                    {
                        RoomData Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
                        if (Data == null)
                        {
                            continue;
                        }

                        if (!Favourites.Contains(Data))
                        {
                            Favourites.Add(Data);
                        }
                    }

                    Favourites = Favourites.Take(FetchLimit).ToList();

                    Message.WriteInteger(Favourites.Count);
                    foreach (RoomData Data in Favourites.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;

                case NavigatorCategoryType.MY_GROUPS:
                    List<RoomData> MyGroups = new List<RoomData>();

                    foreach (int GroupId in Session.GetUser().MyGroups.ToList())
                    {
                        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
                        {
                            continue;
                        }

                        RoomData Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId);
                        if (Data == null)
                        {
                            continue;
                        }

                        if (!MyGroups.Contains(Data))
                        {
                            MyGroups.Add(Data);
                        }
                    }

                    MyGroups = MyGroups.Take(FetchLimit).ToList();

                    Message.WriteInteger(MyGroups.Count);
                    foreach (RoomData Data in MyGroups.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;

                /*case NavigatorCategoryType.MY_FRIENDS_ROOMS:
                    List<RoomData> MyFriendsRooms = new List<RoomData>();
                    foreach (MessengerBuddy buddy in Session.GetUser().GetMessenger().GetFriends().Where(p => p.))
                    {
                        if (buddy == null || !buddy.InRoom || buddy.UserId == Session.GetUser().Id)
                            continue;

                        if (!MyFriendsRooms.Contains(buddy.CurrentRoom.RoomData))
                            MyFriendsRooms.Add(buddy.CurrentRoom.RoomData);
                    }

                    Message.WriteInteger(MyFriendsRooms.Count);
                    foreach (RoomData Data in MyFriendsRooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;*/

                case NavigatorCategoryType.MY_RIGHTS:
                    List<RoomData> MyRights = new List<RoomData>();

                    foreach (int RoomId in Session.GetUser().RoomRightsList)
                    {
                        RoomData Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
                        if (Data == null)
                        {
                            continue;
                        }

                        if (!MyRights.Contains(Data))
                        {
                            MyRights.Add(Data);
                        }
                    }

                    MyRights = MyRights.Take(FetchLimit).ToList();

                    Message.WriteInteger(MyRights.Count);
                    foreach (RoomData Data in MyRights.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;
            }
        }
    }
}