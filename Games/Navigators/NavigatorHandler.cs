namespace WibboEmulator.Games.Navigators;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

internal static class NavigatorHandler
{
    public static void Search(ServerPacket message, SearchResultList searchResult, string searchData, GameClient Session, int fetchLimit)
    {
        //Switching by categorys.
        switch (searchResult.CategoryType)
        {
            default:
                message.WriteInteger(0);
                break;

            case NavigatorCategoryType.Query:
            {
                if (searchData.StartsWith("owner:", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (searchData.Length > 0)
                    {
                        List<RoomEntity> roomList = null;
                        using (var dbClient = DatabaseManager.Connection)
                        {
                            if (searchData.StartsWith("owner:", StringComparison.CurrentCultureIgnoreCase))
                            {
                                roomList = RoomDao.GetAllSearchByUsername(dbClient, searchData.Remove(0, 6));
                            }
                        }

                        var results = new List<RoomData>();
                        if (roomList != null && roomList.Count != 0)
                        {
                            foreach (var room in roomList)
                            {
                                var roomData = RoomManager.FetchRoomData(room.Id, room);
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
                else if (searchData.StartsWith("tag:", StringComparison.CurrentCultureIgnoreCase))
                {
                    searchData = searchData.Remove(0, 4);
                    ICollection<RoomData> tagMatches = RoomManager.SearchTaggedRooms(searchData);

                    message.WriteInteger(tagMatches.Count);
                    foreach (var data in tagMatches.ToList())
                    {
                        RoomAppender.WriteRoom(message, data);
                    }
                }
                else if (searchData.StartsWith("group:", StringComparison.CurrentCultureIgnoreCase))
                {
                    searchData = searchData.Remove(0, 6);
                    ICollection<RoomData> groupRooms = RoomManager.SearchGroupRooms(searchData);

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
                        List<RoomEntity> roomList = null;
                        using (var dbClient = DatabaseManager.Connection)
                        {
                            roomList = RoomDao.GetAllSearch(dbClient, searchData);
                        }

                        var results = new List<RoomData>();
                        if (roomList != null && roomList.Count != 0)
                        {
                            foreach (var room in roomList)
                            {
                                if (room.State == RoomState.Hide)
                                {
                                    continue;
                                }

                                var rData = RoomManager.FetchRoomData(room.Id, room);
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

            case NavigatorCategoryType.Featured:
            case NavigatorCategoryType.FeaturedGame:
            case NavigatorCategoryType.FeaturedNovelty:
            case NavigatorCategoryType.FeaturedHelpSecurity:
            case NavigatorCategoryType.FeaturedRun:
            case NavigatorCategoryType.FeaturedCasino:
                var rooms = new List<RoomData>();
                var featured = NavigatorManager.GetFeaturedRooms(Session.Language);
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

                    var data = RoomManager.GenerateRoomData(featuredItem.RoomId);
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

            case NavigatorCategoryType.Popular:
            {
                var popularRooms = new List<RoomData>();

                popularRooms.AddRange(RoomManager.GetPopularRooms(-1, 20, Session.Language)); //FetchLimit

                message.WriteInteger(popularRooms.Count);
                foreach (var data in popularRooms.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;
            }

            case NavigatorCategoryType.Recommended:
            {
                var recommendedRooms = RoomManager.GetRecommendedRooms(fetchLimit);

                message.WriteInteger(recommendedRooms.Count);
                foreach (var data in recommendedRooms.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;
            }

            case NavigatorCategoryType.Category:
            {
                var getRoomsByCategory = RoomManager.GetRoomsByCategory(searchResult.Id, fetchLimit);

                message.WriteInteger(getRoomsByCategory.Count);
                foreach (var data in getRoomsByCategory.ToList())
                {
                    RoomAppender.WriteRoom(message, data);
                }
                break;
            }

            case NavigatorCategoryType.MyRooms:

                var myRooms = new List<RoomData>();

                foreach (var roomId in Session.User.UsersRooms)
                {
                    var data = RoomManager.GenerateRoomData(roomId);
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

            case NavigatorCategoryType.MyFavorites:
                var favourites = new List<RoomData>();
                foreach (var roomId in Session.User.FavoriteRooms)
                {
                    var data = RoomManager.GenerateRoomData(roomId);
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

            case NavigatorCategoryType.MyGroups:
                var myGroups = new List<RoomData>();

                foreach (var groupId in Session.User.MyGroups.ToList())
                {
                    if (!GroupManager.TryGetGroup(groupId, out var group))
                    {
                        continue;
                    }

                    var data = RoomManager.GenerateRoomData(group.RoomId);
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

            case NavigatorCategoryType.MyRights:
                var myRights = new List<RoomData>();

                foreach (var roomId in Session.User.RoomRightsList)
                {
                    var data = RoomManager.GenerateRoomData(roomId);
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
