using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Rooms;
using Butterfly.Game.User.Messenger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Game.Navigator
{
    internal static class NavigatorHandler
    {
        public static void Search(ServerPacket Message, SearchResultList SearchResult, string SearchData, Client Session, int FetchLimit)
        {
            //Switching by categorys.
            switch (SearchResult.CategoryType)
            {
                default:
                    Message.WriteInteger(0);
                    break;

                case NavigatorCategoryType.QUERY:
                    {
                        #region Query
                        if (SearchData.ToLower().StartsWith("owner:"))
                        {
                            if (SearchData.Length > 0)
                            {
                                DataTable GetRooms = null;
                                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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
                                        RoomData RoomData = ButterflyEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
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
                            ICollection<RoomData> TagMatches = ButterflyEnvironment.GetGame().GetRoomManager().SearchTaggedRooms(SearchData);

                            Message.WriteInteger(TagMatches.Count);
                            foreach (RoomData Data in TagMatches.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data);
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("group:"))
                        {
                            SearchData = SearchData.Remove(0, 6);
                            ICollection<RoomData> GroupRooms = ButterflyEnvironment.GetGame().GetRoomManager().SearchGroupRooms(SearchData);

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
                                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                        RoomData RData = ButterflyEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
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
                        #endregion

                        break;
                    }

                case NavigatorCategoryType.FEATURED:
                    #region Featured
                    List<RoomData> Rooms = new List<RoomData>();
                    ICollection<FeaturedRoom> Featured = ButterflyEnvironment.GetGame().GetNavigator().GetFeaturedRooms(Session.Langue);
                    foreach (FeaturedRoom FeaturedItem in Featured.ToList())
                    {
                        if (FeaturedItem == null)
                        {
                            continue;
                        }

                        if (FeaturedItem.Game)
                        {
                            continue;
                        }

                        RoomData Data = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(FeaturedItem.RoomId);
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
                    #endregion
                    break;

                case NavigatorCategoryType.FEATURED_GAME:
                    #region Featured
                    List<RoomData> GameRooms = new List<RoomData>();
                    ICollection<FeaturedRoom> FeaturedGame = ButterflyEnvironment.GetGame().GetNavigator().GetFeaturedRooms(Session.Langue);
                    foreach (FeaturedRoom FeaturedItem in FeaturedGame.ToList())
                    {
                        if (FeaturedItem == null)
                        {
                            continue;
                        }

                        if (!FeaturedItem.Game)
                        {
                            continue;
                        }

                        RoomData Data = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(FeaturedItem.RoomId);
                        if (Data == null)
                        {
                            continue;
                        }

                        if (!GameRooms.Contains(Data))
                        {
                            GameRooms.Add(Data);
                        }
                    }

                    Message.WriteInteger(GameRooms.Count);
                    foreach (RoomData Data in GameRooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    #endregion
                    break;

                case NavigatorCategoryType.POPULAR:
                    {
                        List<RoomData> PopularRooms = new List<RoomData>();

                        PopularRooms.AddRange(ButterflyEnvironment.GetGame().GetRoomManager().GetPopularRooms(-1, 50, Session.Langue)); //FetchLimit

                        Message.WriteInteger(PopularRooms.Count);
                        foreach (RoomData Data in PopularRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data);
                        }
                        break;
                    }

                case NavigatorCategoryType.RECOMMENDED:
                    {
                        List<RoomData> RecommendedRooms = ButterflyEnvironment.GetGame().GetRoomManager().GetRecommendedRooms(FetchLimit);

                        Message.WriteInteger(RecommendedRooms.Count);
                        foreach (RoomData Data in RecommendedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data);
                        }
                        break;
                    }

                case NavigatorCategoryType.CATEGORY:
                    {
                        List<RoomData> GetRoomsByCategory = ButterflyEnvironment.GetGame().GetRoomManager().GetRoomsByCategory(SearchResult.Id, FetchLimit);

                        Message.WriteInteger(GetRoomsByCategory.Count);
                        foreach (RoomData Data in GetRoomsByCategory.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data);
                        }
                        break;
                    }

                case NavigatorCategoryType.MY_ROOMS:

                    Message.WriteInteger(Session.GetHabbo().UsersRooms.Count);
                    foreach (RoomData Data in Session.GetHabbo().UsersRooms.OrderBy(a => a.Name).ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;

                case NavigatorCategoryType.MY_FAVORITES:
                    List<RoomData> Favourites = new List<RoomData>();
                    foreach (RoomData Room in Session.GetHabbo().FavoriteRooms.ToArray())
                    {

                        if (!Favourites.Contains(Room))
                        {
                            Favourites.Add(Room);
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

                    foreach (int GroupId in Session.GetHabbo().MyGroups.ToList())
                    {
                        if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Guild Group))
                        {
                            continue;
                        }

                        RoomData Data = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId);
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
                    foreach (MessengerBuddy buddy in Session.GetHabbo().GetMessenger().GetFriends().Where(p => p.))
                    {
                        if (buddy == null || !buddy.InRoom || buddy.UserId == Session.GetHabbo().Id)
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

                    foreach (RoomData Room in Session.GetHabbo().RoomRightsList.ToArray())
                    {
                        if (Room == null)
                        {
                            continue;
                        }

                        if (!MyRights.Contains(Room))
                        {
                            MyRights.Add(Room);
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