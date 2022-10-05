namespace WibboEmulator.Games.Navigator;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

internal static class NavigatorHandler
{
    public static void Search(ServerPacket message, SearchResultList searchResult, string searchData, GameClient session, int fetchLimit)
    {
        //Switching by categorys.
        switch (searchResult.CategoryType)
        {
            default:
                message.WriteInteger(0);
                break;

            case NavigatorCategoryType.QUERY:
            {
                if (searchData.ToLower().StartsWith("owner:"))
                {
                    if (searchData.Length > 0)
                    {
                        DataTable getRooms = null;
                        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            if (searchData.ToLower().StartsWith("owner:"))
                            {
                                getRooms = RoomDao.GetAllSearchByUsername(dbClient, searchData.Remove(0, 6));
                            }
                        }

                        var results = new List<RoomData>();
                        if (getRooms != null)
                        {
                            foreach (DataRow row in getRooms.Rows)
                            {
                                var roomData = WibboEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(row["id"]), row);
                                if (roomData != null && !results.Contains(roomData))
                                {
                                    results.Add(roomData);
                                }
                            }
                        }

                        message.WriteInteger(results.Count);
                        foreach (var Data in results.ToList())
                        {
                            RoomAppender.WriteRoom(message, Data);
                        }
                    }
                }
                else if (searchData.ToLower().StartsWith("tag:"))
                {
                    searchData = searchData.Remove(0, 4);
                    ICollection<RoomData> TagMatches = WibboEnvironment.GetGame().GetRoomManager().SearchTaggedRooms(searchData);

                    message.WriteInteger(TagMatches.Count);
                    foreach (var Data in TagMatches.ToList())
                    {
                        RoomAppender.WriteRoom(message, Data);
                    }
                }
                else if (searchData.ToLower().StartsWith("group:"))
                {
                    searchData = searchData.Remove(0, 6);
                    ICollection<RoomData> GroupRooms = WibboEnvironment.GetGame().GetRoomManager().SearchGroupRooms(searchData);

                    message.WriteInteger(GroupRooms.Count);
                    foreach (var Data in GroupRooms.ToList())
                    {
                        RoomAppender.WriteRoom(message, Data);
                    }
                }
                else
                {
                    if (searchData.Length > 0)
                    {
                        DataTable Table = null;
                        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            Table = RoomDao.GetAllSearch(dbClient, searchData);
                        }

                        var Results = new List<RoomData>();
                        if (Table != null)
                        {
                            foreach (DataRow Row in Table.Rows)
                            {
                                if (Convert.ToString(Row["state"]) == "invisible")
                                {
                                    continue;
                                }

                                var RData = WibboEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
                                if (RData != null && !Results.Contains(RData))
                                {
                                    Results.Add(RData);
                                }
                            }
                        }

                        message.WriteInteger(Results.Count);
                        foreach (var Data in Results.ToList())
                        {
                            RoomAppender.WriteRoom(message, Data);
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
                var Rooms = new List<RoomData>();
                var Featured = WibboEnvironment.GetGame().GetNavigator().GetFeaturedRooms(session.Langue);
                foreach (var FeaturedItem in Featured.ToList())
                {
                    if (FeaturedItem == null)
                    {
                        continue;
                    }

                    if (FeaturedItem.CategoryType != searchResult.CategoryType)
                    {
                        continue;
                    }

                    var Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(FeaturedItem.RoomId);
                    if (Data == null)
                    {
                        continue;
                    }

                    if (!Rooms.Contains(Data))
                    {
                        Rooms.Add(Data);
                    }
                }

                message.WriteInteger(Rooms.Count);
                foreach (var Data in Rooms.ToList())
                {
                    RoomAppender.WriteRoom(message, Data);
                }
                break;

            case NavigatorCategoryType.POPULAR:
            {
                var PopularRooms = new List<RoomData>();

                PopularRooms.AddRange(WibboEnvironment.GetGame().GetRoomManager().GetPopularRooms(-1, 50, session.Langue)); //FetchLimit

                message.WriteInteger(PopularRooms.Count);
                foreach (var Data in PopularRooms.ToList())
                {
                    RoomAppender.WriteRoom(message, Data);
                }
                break;
            }

            case NavigatorCategoryType.RECOMMENDED:
            {
                var RecommendedRooms = WibboEnvironment.GetGame().GetRoomManager().GetRecommendedRooms(fetchLimit);

                message.WriteInteger(RecommendedRooms.Count);
                foreach (var Data in RecommendedRooms.ToList())
                {
                    RoomAppender.WriteRoom(message, Data);
                }
                break;
            }

            case NavigatorCategoryType.CATEGORY:
            {
                var GetRoomsByCategory = WibboEnvironment.GetGame().GetRoomManager().GetRoomsByCategory(searchResult.Id, fetchLimit);

                message.WriteInteger(GetRoomsByCategory.Count);
                foreach (var Data in GetRoomsByCategory.ToList())
                {
                    RoomAppender.WriteRoom(message, Data);
                }
                break;
            }

            case NavigatorCategoryType.MY_ROOMS:

                var MyRooms = new List<RoomData>();

                foreach (var RoomId in session.GetUser().UsersRooms)
                {
                    var Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
                    if (Data == null)
                    {
                        continue;
                    }

                    if (!MyRooms.Contains(Data))
                    {
                        MyRooms.Add(Data);
                    }
                }

                message.WriteInteger(MyRooms.Count);
                foreach (var Data in MyRooms.OrderBy(a => a.Name).ToList())
                {
                    RoomAppender.WriteRoom(message, Data);
                }
                break;

            case NavigatorCategoryType.MY_FAVORITES:
                var Favourites = new List<RoomData>();
                foreach (var RoomId in session.GetUser().FavoriteRooms)
                {
                    var Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
                    if (Data == null)
                    {
                        continue;
                    }

                    if (!Favourites.Contains(Data))
                    {
                        Favourites.Add(Data);
                    }
                }

                Favourites = Favourites.Take(fetchLimit).ToList();

                message.WriteInteger(Favourites.Count);
                foreach (var Data in Favourites.ToList())
                {
                    RoomAppender.WriteRoom(message, Data);
                }
                break;

            case NavigatorCategoryType.MY_GROUPS:
                var MyGroups = new List<RoomData>();

                foreach (var GroupId in session.GetUser().MyGroups.ToList())
                {
                    if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
                    {
                        continue;
                    }

                    var Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId);
                    if (Data == null)
                    {
                        continue;
                    }

                    if (!MyGroups.Contains(Data))
                    {
                        MyGroups.Add(Data);
                    }
                }

                MyGroups = MyGroups.Take(fetchLimit).ToList();

                message.WriteInteger(MyGroups.Count);
                foreach (var Data in MyGroups.ToList())
                {
                    RoomAppender.WriteRoom(message, Data);
                }
                break;

            /*case NavigatorCategoryType.MY_FRIENDS_ROOMS:
                List<RoomData> MyFriendsRooms = new List<RoomData>();
                foreach (MessengerBuddy buddy in session.GetUser().GetMessenger().GetFriends().Where(p => p.))
                {
                    if (buddy == null || !buddy.InRoom || buddy.UserId == session.GetUser().Id)
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
                var MyRights = new List<RoomData>();

                foreach (var RoomId in session.GetUser().RoomRightsList)
                {
                    var Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
                    if (Data == null)
                    {
                        continue;
                    }

                    if (!MyRights.Contains(Data))
                    {
                        MyRights.Add(Data);
                    }
                }

                MyRights = MyRights.Take(fetchLimit).ToList();

                message.WriteInteger(MyRights.Count);
                foreach (var Data in MyRights.ToList())
                {
                    RoomAppender.WriteRoom(message, Data);
                }
                break;
        }
    }
}
