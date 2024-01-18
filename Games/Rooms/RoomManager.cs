namespace WibboEmulator.Games.Rooms;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Room;

public class RoomManager
{
    private readonly object _roomLoadingSync;

    private readonly ConcurrentDictionary<int, Room> _rooms;
    private readonly Dictionary<string, RoomModel> _roomModels;
    private readonly ConcurrentDictionary<int, RoomData> _roomsData;

    public int Count => this._rooms.Count;

    public RoomManager()
    {
        this._roomLoadingSync = new object();

        this._rooms = new ConcurrentDictionary<int, Room>();
        this._roomModels = new Dictionary<string, RoomModel>();
        this._roomsData = new ConcurrentDictionary<int, RoomData>();

        this._roomCycleStopwatch = new Stopwatch();
        this._roomCycleStopwatch.Start();
    }

    private static RoomModel GetCustomData(int roomID)
    {
        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        var modelCustom = RoomModelCustomDao.GetOne(dbClient, roomID) ?? throw new ArgumentNullException("The custom room model for room " + roomID + " was not found");

        return new RoomModel(roomID.ToString(), modelCustom.DoorX, modelCustom.DoorY, modelCustom.DoorZ, modelCustom.DoorDir, modelCustom.Heightmap, modelCustom.WallHeight);
    }

    public RoomModel GetModel(string model, int roomID)
    {
        if (model == "model_custom")
        {
            return GetCustomData(roomID);
        }

        if (this._roomModels.TryGetValue(model, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    public RoomData GenerateNullableRoomData(int roomId)
    {
        if (this.GenerateRoomData(roomId) != null)
        {
            return this.GenerateRoomData(roomId);
        }

        var roomData = new RoomData();
        roomData.FillNull(roomId);
        return roomData;
    }

    public RoomData GenerateRoomData(int roomId)
    {
        if (this.TryGetRoom(roomId, out var room))
        {
            return room.RoomData;
        }

        if (this.TryGetRoomData(roomId, out var roomData))
        {
            return roomData;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        var roomEntity = RoomDao.GetOne(dbClient, roomId);

        if (roomEntity == null)
        {
            return null;
        }

        roomData = new RoomData();
        roomData.Fill(roomEntity);

        if (!this._roomsData.ContainsKey(roomId))
        {
            _ = this._roomsData.TryAdd(roomId, roomData);
        }

        return roomData;
    }

    public Room LoadRoom(int id)
    {
        if (this.TryGetRoom(id, out var room))
        {
            return room;
        }

        lock (this._roomLoadingSync)
        {
            var data = this.GenerateRoomData(id);
            if (data == null)
            {
                return null;
            }

            room = new Room(data);

            if (!this._rooms.ContainsKey(room.Id))
            {
                _ = this._rooms.TryAdd(room.Id, room);
            }

            if (this._roomsData.ContainsKey(room.Id))
            {
                _ = this._roomsData.TryRemove(room.Id, out data);
            }

            return room;
        }
    }

    public void RoomDataRemove(int id)
    {
        if (this._roomsData.ContainsKey(id))
        {
            _ = this._roomsData.TryRemove(id, out _);
        }
    }

    public bool TryGetRoom(int roomId, out Room room) => this._rooms.TryGetValue(roomId, out room);

    public bool TryGetRoomModels(string model, out RoomModel roomModel) => this._roomModels.TryGetValue(model, out roomModel);

    public bool TryGetRoomData(int roomId, out RoomData roomData) => this._roomsData.TryGetValue(roomId, out roomData);

    public RoomData FetchRoomData(int roomID, RoomEntity roomEntity)
    {
        if (this.TryGetRoom(roomID, out var room))
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

    public void Init(IDbConnection dbClient)
    {
        this._roomModels.Clear();

        var roomMoodelList = RoomModelDao.GetAll(dbClient);
        if (roomMoodelList.Count == 0)
        {
            return;
        }

        foreach (var roomMoodel in roomMoodelList)
        {
            this._roomModels.Add(roomMoodel.Id, new RoomModel(roomMoodel.Id, roomMoodel.DoorX, roomMoodel.DoorY, roomMoodel.DoorZ, roomMoodel.DoorDir, roomMoodel.Heightmap, 0));
        }
    }

    public void OnCycle(Stopwatch moduleWatch)
    {
        this.RoomCycleTask();
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

    public List<RoomData> SearchGroupRooms(string query, int amount = 20)
    {
        var instanceMatches =
            (from RoomInstance in this._rooms.ToList()
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

    public List<RoomData> SearchTaggedRooms(string query)
    {
        var instanceMatches =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.RoomData.UsersNow >= 0 &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible &&
             RoomInstance.Value.RoomData.Tags.Contains(query)
             orderby RoomInstance.Value.RoomData.UsersNow descending
             select RoomInstance.Value.RoomData).Take(50);
        return instanceMatches.ToList();
    }

    public List<RoomData> GetPopularRooms(int category, int amount = 20, Language langue = Language.French)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value != null && RoomInstance.Value.RoomData != null &&
             RoomInstance.Value.RoomData.UsersNow > 0 &&
             (category == -1 || RoomInstance.Value.RoomData.Category == category) &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible && RoomInstance.Value.RoomData.Langue == langue
             orderby RoomInstance.Value.RoomData.Score descending
             orderby RoomInstance.Value.RoomData.UsersNow descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    public List<RoomData> GetRecommendedRooms(int amount = 20, int currentRoomId = 0)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.RoomData.UsersNow >= 0 &&
             RoomInstance.Value.RoomData.Score >= 0 &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible &&
             RoomInstance.Value.RoomData.Id != currentRoomId
             orderby RoomInstance.Value.RoomData.Score descending
             orderby RoomInstance.Value.RoomData.UsersNow descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    public List<RoomData> GetPopularRatedRooms(int amount = 20)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.RoomData.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.RoomData.Score descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    public List<RoomData> GetRoomsByCategory(int category, int amount = 20)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.RoomData.Category == category &&
             RoomInstance.Value.RoomData.UsersNow > 0 &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.RoomData.UsersNow descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    public List<KeyValuePair<string, int>> GetPopularRoomTags(int amount = 20)
    {
        var tags =
            (from RoomInstance in this._rooms.ToList()
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
                if (!tagValues.ContainsKey(tag))
                {
                    tagValues.Add(tag, 1);
                }
                else
                {
                    tagValues[tag]++;
                }
            }
        }

        var sortedTags = new List<KeyValuePair<string, int>>(tagValues);
        sortedTags.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));

        sortedTags.Reverse();
        return sortedTags;
    }

    public List<RoomData> GetGroupRooms(int amount = 20)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.RoomData.Group != null &&
             RoomInstance.Value.RoomData.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.RoomData.Score descending
             select RoomInstance.Value.RoomData).Take(amount);
        return rooms.ToList();
    }

    private readonly Stopwatch _roomCycleStopwatch;
    public void RoomCycleTask()
    {
        if (this._roomCycleStopwatch.ElapsedMilliseconds >= 500)
        {
            this._roomCycleStopwatch.Restart();

            foreach (var room in this._rooms.ToList())
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

                        this.UnloadRoom(room.Value);
                    }
                }
            }
        }
    }

    public void RemoveAllRooms()
    {
        var count = this._rooms.Count;
        var num = 0;

        foreach (var room in this._rooms.Values.ToList())
        {
            if (room == null)
            {
                continue;
            }

            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
            Console.Clear();
            Console.WriteLine("<<- SERVER SHUTDOWN ->> ROOM ITEM SAVE: " + string.Format("{0:0.##}", num / (double)count * 100.0) + "%");
            num++;
        }

        Console.WriteLine("Done disposing rooms!");
    }

    public void UnloadEmptyRooms()
    {
        foreach (var room in this._rooms.Values.ToList())
        {
            if (room == null)
            {
                continue;
            }

            if (room.UserCount > 0)
            {
                continue;
            }

            this.UnloadRoom(room);
        }
    }

    public void UnloadRoom(Room room)
    {
        if (room == null)
        {
            return;
        }

        if (this._rooms.TryRemove(room.Id, out _))
        {
            ((IDisposable)room).Dispose();
        }
    }
}
