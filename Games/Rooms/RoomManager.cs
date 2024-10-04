namespace WibboEmulator.Games.Rooms;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;

public static class RoomManager
{
    private static readonly object RoomLoadingSync = new();

    private static readonly ConcurrentDictionary<int, Room> Rooms = new();
    private static readonly Dictionary<string, RoomModel> RoomModels = [];
    private static readonly ConcurrentDictionary<int, RoomData> RoomsData = new();

    public static int Count => Rooms.Count;

    private static RoomModel GetCustomData(int roomID)
    {
        using var dbClient = DatabaseManager.Connection;
        var modelCustom = RoomModelCustomDao.GetOne(dbClient, roomID) ?? throw new ArgumentNullException("The custom room model for room " + roomID + " was not found");

        return new RoomModel(roomID.ToString(), modelCustom.DoorX, modelCustom.DoorY, modelCustom.DoorZ, modelCustom.DoorDir, modelCustom.Heightmap, modelCustom.WallHeight);
    }

    public static RoomModel GetModel(string model, int roomID)
    {
        if (model == "model_custom")
        {
            return GetCustomData(roomID);
        }

        if (RoomModels.TryGetValue(model, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    public static RoomData GenerateNullableRoomData(int roomId)
    {
        if (GenerateRoomData(roomId) != null)
        {
            return GenerateRoomData(roomId);
        }

        var roomData = new RoomData();
        roomData.FillNull(roomId);
        return roomData;
    }

    public static RoomData GenerateRoomData(int roomId)
    {
        if (TryGetRoom(roomId, out var room))
        {
            return room.RoomData;
        }

        if (TryGetRoomData(roomId, out var roomData))
        {
            return roomData;
        }

        using var dbClient = DatabaseManager.Connection;
        var roomEntity = RoomDao.GetOne(dbClient, roomId);

        if (roomEntity == null)
        {
            return null;
        }

        roomData = new RoomData();
        roomData.Fill(roomEntity);

        if (!RoomsData.ContainsKey(roomId))
        {
            _ = RoomsData.TryAdd(roomId, roomData);
        }

        return roomData;
    }

    public static Room LoadRoom(int id)
    {
        if (TryGetRoom(id, out var room))
        {
            return room;
        }

        lock (RoomLoadingSync)
        {
            var data = GenerateRoomData(id);
            if (data == null)
            {
                return null;
            }

            room = new Room(data);

            if (!Rooms.ContainsKey(room.Id))
            {
                _ = Rooms.TryAdd(room.Id, room);
            }

            if (RoomsData.ContainsKey(room.Id))
            {
                _ = RoomsData.TryRemove(room.Id, out data);
            }

            return room;
        }
    }

    public static void RoomDataRemove(int id)
    {
        if (RoomsData.ContainsKey(id))
        {
            _ = RoomsData.TryRemove(id, out _);
        }
    }

    public static bool TryGetRoom(int roomId, out Room room) => Rooms.TryGetValue(roomId, out room);

    public static bool TryGetRoomModels(string model, out RoomModel roomModel) => RoomModels.TryGetValue(model, out roomModel);

    public static bool TryGetRoomData(int roomId, out RoomData roomData) => RoomsData.TryGetValue(roomId, out roomData);

    public static RoomData FetchRoomData(int roomID, RoomEntity roomEntity)
    {
        if (TryGetRoom(roomID, out var room))
        {
            return room.RoomData;
        }
        else
        {
            var data = new RoomData();
            data.Fill(roomEntity);
            return data;
        }
    }

    public static void Initialize(IDbConnection dbClient)
    {
        RoomCycleStopwatch.Start();

        RoomModels.Clear();

        var roomMoodelList = RoomModelDao.GetAll(dbClient);
        if (roomMoodelList.Count == 0)
        {
            return;
        }

        foreach (var roomMoodel in roomMoodelList)
        {
            RoomModels.Add(roomMoodel.Id, new RoomModel(roomMoodel.Id, roomMoodel.DoorX, roomMoodel.DoorY, roomMoodel.DoorZ, roomMoodel.DoorDir, roomMoodel.Heightmap, 0));
        }
    }

    public static void OnCycle(Stopwatch moduleWatch)
    {
        RoomCycleTask();
        HandleFunctionReset(moduleWatch, "RoomCycleTask");
    }

    private static void HandleFunctionReset(Stopwatch watch, string methodName)
    {
        try
        {
            if (watch.ElapsedMilliseconds > 500)
            {
                Console.WriteLine("High latency in {0}: {1}ms", methodName, watch.ElapsedMilliseconds);
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine("Canceled operation {0}", e);

        }
        watch.Restart();
    }

    public static List<RoomData> SearchGroupRooms(string query, int amount = 20)
    {
        var instanceMatches =
            (from RoomInstance in Rooms.ToList()
             where RoomInstance.Value.RoomData.UsersNow >= 0 &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible &&
             RoomInstance.Value.RoomData.Group != null &&
             (RoomInstance.Value.RoomData.OwnerName.StartsWith(query) ||
             RoomInstance.Value.RoomData.Tags.Contains(query) ||
             RoomInstance.Value.RoomData.Name.Contains(query))
             orderby RoomInstance.Value.RoomData.UsersNow descending
             select RoomInstance.Value.RoomData).Take(amount);
        return instanceMatches.ToList();
    }

    public static List<RoomData> SearchTaggedRooms(string query)
    {
        var instanceMatches =
            (from RoomInstance in Rooms.ToList()
             where RoomInstance.Value.RoomData.UsersNow >= 0 &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible &&
             RoomInstance.Value.RoomData.Tags.Contains(query)
             orderby RoomInstance.Value.RoomData.UsersNow descending
             select RoomInstance.Value.RoomData).Take(50);
        return instanceMatches.ToList();
    }

    public static List<RoomData> GetPopularRooms(int category, int amount = 20, Language langue = Language.French)
    {
        var rooms =
            (from RoomInstance in Rooms.ToList()
             where RoomInstance.Value != null && RoomInstance.Value.RoomData != null &&
             RoomInstance.Value.RoomData.UsersNow > 0 &&
             (category == -1 || RoomInstance.Value.RoomData.Category == category) &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible && RoomInstance.Value.RoomData.Language == langue
             orderby RoomInstance.Value.RoomData.Score descending
             orderby RoomInstance.Value.RoomData.UsersNow descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    public static List<RoomData> GetRecommendedRooms(int amount = 20, int currentRoomId = 0)
    {
        var rooms =
            (from RoomInstance in Rooms.ToList()
             where RoomInstance.Value.RoomData.UsersNow >= 0 &&
             RoomInstance.Value.RoomData.Score >= 0 &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible &&
             RoomInstance.Value.RoomData.Id != currentRoomId
             orderby RoomInstance.Value.RoomData.Score descending
             orderby RoomInstance.Value.RoomData.UsersNow descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    public static List<RoomData> GetPopularRatedRooms(int amount = 20)
    {
        var rooms =
            (from RoomInstance in Rooms.ToList()
             where RoomInstance.Value.RoomData.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.RoomData.Score descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    public static List<RoomData> GetRoomsByCategory(int category, int amount = 20)
    {
        var rooms =
            (from RoomInstance in Rooms.ToList()
             where RoomInstance.Value.RoomData.Category == category &&
             RoomInstance.Value.RoomData.UsersNow > 0 &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.RoomData.UsersNow descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    public static List<KeyValuePair<string, int>> GetPopularRoomTags(int amount = 20)
    {
        var tags =
            (from RoomInstance in Rooms.ToList()
             where RoomInstance.Value.RoomData.UsersNow >= 0 &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.RoomData.UsersNow descending
             orderby RoomInstance.Value.RoomData.Score descending
             select RoomInstance.Value.RoomData.Tags).Take(amount);

        var tagValues = new Dictionary<string, int>();

        foreach (var tagList in tags)
        {
            foreach (var tag in tagList)
            {
                if (!tagValues.TryGetValue(tag, out var value))
                {
                    tagValues.Add(tag, 1);
                }
                else
                {
                    tagValues[tag] = ++value;
                }
            }
        }

        var sortedTags = new List<KeyValuePair<string, int>>(tagValues);
        sortedTags.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));

        sortedTags.Reverse();
        return sortedTags;
    }

    public static List<RoomData> GetGroupRooms(int amount = 20)
    {
        var rooms =
            (from RoomInstance in Rooms.ToList()
             where RoomInstance.Value.RoomData.Group != null &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.RoomData.Score descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    private static readonly Stopwatch RoomCycleStopwatch = new();
    public static void RoomCycleTask()
    {
        if (RoomCycleStopwatch.ElapsedMilliseconds >= 500)
        {
            RoomCycleStopwatch.Restart();

            foreach (var room in Rooms)
            {
                if (room.Value == null)
                {
                    continue;
                }

                if (room.Value.ProcessTask == null || room.Value.ProcessTask.IsCompleted)
                {
                    room.Value.ProcessTask = room.Value.RunTask(room.Value.ProcessRoom);

                    room.Value.IsLagging = 0;
                }
                else
                {
                    room.Value.IsLagging++;
                    if (room.Value.IsLagging > 20)
                    {
                        ExceptionLogger.LogThreadException("Room lagging", "Room cycle task for room " + room.Value.Id);

                        UnloadRoom(room.Value);
                    }
                }
            }
        }
    }

    public static void RemoveAllRooms()
    {
        var count = Rooms.Count;
        var num = 0;

        foreach (var room in Rooms.Values)
        {
            if (room == null)
            {
                continue;
            }

            UnloadRoom(room);
            Console.Clear();
            Console.WriteLine("<<- SERVER SHUTDOWN ->> ROOM ITEM SAVE: " + string.Format("{0:0.##}", num / (double)count * 100.0) + "%");
            num++;
        }

        Console.WriteLine("Done disposing rooms!");
    }

    public static void UnloadEmptyRooms()
    {
        foreach (var room in Rooms.Values)
        {
            if (room == null)
            {
                continue;
            }

            if (room.UserCount > 0)
            {
                continue;
            }

            UnloadRoom(room);
        }
    }

    public static void UnloadRoom(Room room)
    {
        if (room == null)
        {
            return;
        }

        if (Rooms.TryRemove(room.Id, out _))
        {
            ((IDisposable)room).Dispose();
        }
    }

        /* public List<Room> SearchGroupRooms(string query)
        {
            return _rooms.Values.Where(x => x.Group != null && x.Group.Name.ToLower().Contains(query.ToLower()) && x.Access != RoomAccess.Invisible).OrderByDescending(x => x.UsersNow).Take(50).ToList();
        }

        public List<Room> SearchTaggedRooms(string query)
        {
            return _rooms.Values.Where(x => x.Tags.Contains(query) && x.Access != RoomAccess.Invisible).OrderByDescending(x => x.UsersNow).Take(50).ToList();
        }

        public List<Room> GetPopularRooms(int category, int amount = 50)
        {
            return _rooms.Values.Where(x => x.UsersNow > 0 && x.Access != RoomAccess.Invisible).OrderByDescending(x => x.UsersNow).Take(amount).ToList();
        }

        public List<Room> GetRecommendedRooms(int amount = 50, int CurrentRoomId = 0)
        {
            return _rooms.Values.Where(x => x.Id != CurrentRoomId && x.Access != RoomAccess.Invisible).OrderByDescending(x => x.UsersNow).OrderByDescending(x => x.Score).Take(amount).ToList();
        }

        public List<Room> GetPopularRatedRooms(int amount = 50)
        {
            return _rooms.Values.Where(x => x.Access != RoomAccess.Invisible).OrderByDescending(x => x.Score).OrderByDescending(x => x.UsersNow).Take(amount).ToList();
        }

        public List<Room> GetRoomsByCategory(int category, int amount = 50)
        {
            return _rooms.Values.Where(x => x.Category == category && x.Access != RoomAccess.Invisible && x.UsersNow > 0).OrderByDescending(x => x.UsersNow).Take(amount).ToList();
        }

        public List<Room> GetOnGoingRoomPromotions(int Mode, int Amount = 50)
        {
            if (Mode == 17)
            {
                return _rooms.Values.Where(x => x.HasActivePromotion && x.Access != RoomAccess.Invisible).OrderByDescending(x => x.Promotion.TimestampStarted).Take(Amount).ToList();
            }

            return _rooms.Values.Where(x => x.HasActivePromotion && x.Access != RoomAccess.Invisible).OrderByDescending(x => x.UsersNow).Take(Amount).ToList();
        }

        public List<Room> GetPromotedRooms(int categoryId, int amount = 50)
        {
            return _rooms.Values.Where(x => x.HasActivePromotion && x.Promotion.CategoryId == categoryId && x.Access != RoomAccess.Invisible).OrderByDescending(x => x.Promotion.TimestampStarted).Take(amount).ToList();
        }

        public List<Room> GetGroupRooms(int amount = 50)
        {
            return _rooms.Values.Where(x => x.Group != null && x.Access != RoomAccess.Invisible).OrderByDescending(x => x.Score).Take(amount).ToList();
        }

        public List<Room> GetRoomsByIds(List<int> ids, int amount = 50)
        {
            return _rooms.Values.Where(x => ids.Contains(x.Id) && x.Access != RoomAccess.Invisible).OrderByDescending(x => x.UsersNow).Take(amount).ToList();
        }

        public Room TryGetRandomLoadedRoom()
        {
            return _rooms.Values.Where(x => x.UsersNow > 0 && x.Access != RoomAccess.Invisible && x.UsersNow < x.UsersMax).OrderByDescending(x => x.UsersNow).FirstOrDefault();
        }
        */
}
