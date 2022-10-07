namespace WibboEmulator.Games.Navigator;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
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
                        foreach (var data in results.ToList())
                        {
                            RoomAppender.WriteRoom(message, data);
                        }
                    }
                }
                else if (searchData.ToLower().StartsWith("tag:"))
                {
                    searchData = searchData.Remove(0, 4);
                    ICollection<RoomData> tagMatches = WibboEnvironment.GetGame().GetRoomManager().SearchTaggedRooms(searchData);

                    message.WriteInteger(tagMatches.Count);
                    foreach (var data in tagMatches.ToList())
                    {
                        RoomAppender.WriteRoom(message, data);
                    }
                }
                else if (searchData.ToLower().StartsWith("group:"))
                {
                    searchData = searchData.Remove(0, 6);
                    ICollection<RoomData> groupRooms = WibboEnvironment.GetGame().GetRoomManager().SearchGroupRooms(searchData);

                    message.WriteInteger(groupRooms.Count);
                    foreach (var data in groupRooms.ToList())
                    {
                        RoomAppender.WriteRoom(message, data);
                    }
                }
                else
                {
                    if (searchData.Length > 0)
                    {
                        DataTable table = null;
                        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            table = RoomDao.GetAllSearch(dbClient, searchData);
                        }

                        var results = new List<RoomData>();
                        if (table != null)
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                if (Convert.ToString(row["state"]) == "invisible")
                                {
                                    continue;
                                }

                                var rData = WibboEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(row["id"]), row);
                                if (rData != null && !results.Contains(rData))
                                {
                                    results.Add(rData);
                                }
                            }
                        }

                        message.WriteInteger(results.Count);
                        foreach (var data in results.ToList())
                        {
                            RoomAppender.WriteRoom(message, data);
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
                var rooms = new List<RoomData>();
                var featured = WibboEnvironment.GetGame().GetNavigator().GetFeaturedRooms(session.Langue);
                foreach (var featuredItem in featured.ToList())
                {
                    if (featuredItem == null)
                    {
                        continue;
                    }

                    if (featuredItem.CategoryType != searchResult.CategoryType)
                    {
                        continue;
                    }

                    var data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(featuredItem.RoomId);
                    if (data == null)
                    {
                        continue;
                    }

                    if (!rooms.Contains(data))
                    {
                        rooms.Add(data);
                    }
                }

                message.WriteInteger(rooms.Count);
                foreach (var data in rooms.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;

            case NavigatorCategoryType.POPULAR:
            {
                var popularRooms = new List<RoomData>();

                popularRooms.AddRange(WibboEnvironment.GetGame().GetRoomManager().GetPopularRooms(-1, 50, session.Langue)); //FetchLimit

                message.WriteInteger(popularRooms.Count);
                foreach (var data in popularRooms.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;
            }

            case NavigatorCategoryType.RECOMMENDED:
            {
                var recommendedRooms = WibboEnvironment.GetGame().GetRoomManager().GetRecommendedRooms(fetchLimit);

                message.WriteInteger(recommendedRooms.Count);
                foreach (var data in recommendedRooms.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;
            }

            case NavigatorCategoryType.CATEGORY:
            {
                var getRoomsByCategory = WibboEnvironment.GetGame().GetRoomManager().GetRoomsByCategory(searchResult.Id, fetchLimit);

                message.WriteInteger(getRoomsByCategory.Count);
                foreach (var data in getRoomsByCategory.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;
            }

            case NavigatorCategoryType.MY_ROOMS:

                var myRooms = new List<RoomData>();

                foreach (var roomId in session.GetUser().UsersRooms)
                {
                    var data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
                    if (data == null)
                    {
                        continue;
                    }

                    if (!myRooms.Contains(data))
                    {
                        myRooms.Add(data);
                    }
                }

                message.WriteInteger(myRooms.Count);
                foreach (var data in myRooms.OrderBy(a => a.Name).ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;

            case NavigatorCategoryType.MY_FAVORITES:
                var favourites = new List<RoomData>();
                foreach (var roomId in session.GetUser().FavoriteRooms)
                {
                    var data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
                    if (data == null)
                    {
                        continue;
                    }

                    if (!favourites.Contains(data))
                    {
                        favourites.Add(data);
                    }
                }

                favourites = favourites.Take(fetchLimit).ToList();

                message.WriteInteger(favourites.Count);
                foreach (var data in favourites.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;

            case NavigatorCategoryType.MY_GROUPS:
                var myGroups = new List<RoomData>();

                foreach (var groupId in session.GetUser().MyGroups.ToList())
                {
                    if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
                    {
                        continue;
                    }

                    var data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(group.RoomId);
                    if (data == null)
                    {
                        continue;
                    }

                    if (!myGroups.Contains(data))
                    {
                        myGroups.Add(data);
                    }
                }

                myGroups = myGroups.Take(fetchLimit).ToList();

                message.WriteInteger(myGroups.Count);
                foreach (var data in myGroups.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
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
                var myRights = new List<RoomData>();

                foreach (var roomId in session.GetUser().RoomRightsList)
                {
                    var data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
                    if (data == null)
                    {
                        continue;
                    }

                    if (!myRights.Contains(data))
                    {
                        myRights.Add(data);
                    }
                }

                myRights = myRights.Take(fetchLimit).ToList();

                message.WriteInteger(myRights.Count);
                foreach (var data in myRights.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;
        }
    }
}
