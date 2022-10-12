namespace WibboEmulator.Games.Rooms;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Interfaces;

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
        DataRow row;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            row = RoomModelCustomDao.GetOne(dbClient, roomID);
        }

        if (row == null)
        {
            throw new ArgumentNullException("The custom room model for room " + roomID + " was not found");
        }

        return new RoomModel(roomID.ToString(), Convert.ToInt32(row["door_x"]), Convert.ToInt32(row["door_y"]), (double)row["door_z"], Convert.ToInt32(row["door_dir"]), (string)row["heightmap"], Convert.ToInt32(row["wall_height"]));
    }

    public RoomModel GetModel(string model, int roomID)
    {
        if (model == "model_custom")
        {
            return GetCustomData(roomID);
        }

        if (this._roomModels.ContainsKey(model))
        {
            return this._roomModels[model];
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
            return room.Data;
        }

        if (this.TryGetRoomData(roomId, out var roomData))
        {
            return roomData;
        }

        DataRow row = null;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            row = RoomDao.GetOne(dbClient, roomId);
        }

        if (row == null)
        {
            return null;
        }

        roomData = new RoomData();
        roomData.Fill(row);

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

    public RoomData FetchRoomData(int roomID, DataRow dRow)
    {
        if (this.TryGetRoom(roomID, out var room))
        {
            return room.Data;
        }
        else
        {
            var data = new RoomData();
            data.Fill(dRow);
            return data;
        }
    }

    public void Init(IQueryAdapter dbClient)
    {
        this._roomModels.Clear();

        var roomMoodelData = RoomModelDao.GetAll(dbClient);
        if (roomMoodelData == null)
        {
            return;
        }

        foreach (DataRow dataRow in roomMoodelData.Rows)
        {
            var str = (string)dataRow["id"];
            this._roomModels.Add(str, new RoomModel(str, Convert.ToInt32(dataRow["door_x"]), Convert.ToInt32(dataRow["door_y"]), (double)dataRow["door_z"], Convert.ToInt32(dataRow["door_dir"]), (string)dataRow["heightmap"], 0));
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

    public List<RoomData> SearchGroupRooms(string query)
    {
        var instanceMatches =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.Data.UsersNow >= 0 &&
             RoomInstance.Value.Data.Access != RoomAccess.Invisible &&
             RoomInstance.Value.Data.Group != null &&
             (RoomInstance.Value.Data.OwnerName.StartsWith(query) ||
             RoomInstance.Value.Data.Tags.Contains(query) ||
             RoomInstance.Value.Data.Name.Contains(query))
             orderby RoomInstance.Value.Data.UsersNow descending
             select RoomInstance.Value.Data).Take(50);
        return instanceMatches.ToList();
    }

    public List<RoomData> SearchTaggedRooms(string query)
    {
        var instanceMatches =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.Data.UsersNow >= 0 &&
             RoomInstance.Value.Data.Access != RoomAccess.Invisible &&
             RoomInstance.Value.Data.Tags.Contains(query)
             orderby RoomInstance.Value.Data.UsersNow descending
             select RoomInstance.Value.Data).Take(50);
        return instanceMatches.ToList();
    }

    public List<RoomData> GetPopularRooms(int category, int amount = 50, Language langue = Language.French)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value != null && RoomInstance.Value.Data != null &&
             RoomInstance.Value.Data.UsersNow > 0 &&
             (category == -1 || RoomInstance.Value.Data.Category == category) &&
             RoomInstance.Value.Data.Access != RoomAccess.Invisible && RoomInstance.Value.Data.Langue == langue
             orderby RoomInstance.Value.Data.Score descending
             orderby RoomInstance.Value.Data.UsersNow descending
             select RoomInstance.Value.Data).Take(amount);
        return rooms.ToList();
    }

    public List<RoomData> GetRecommendedRooms(int amount = 50, int currentRoomId = 0)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.Data.UsersNow >= 0 &&
             RoomInstance.Value.Data.Score >= 0 &&
             RoomInstance.Value.Data.Access != RoomAccess.Invisible &&
             RoomInstance.Value.Data.Id != currentRoomId
             orderby RoomInstance.Value.Data.Score descending
             orderby RoomInstance.Value.Data.UsersNow descending
             select RoomInstance.Value.Data).Take(amount);
        return rooms.ToList();
    }

    public List<RoomData> GetPopularRatedRooms(int amount = 50)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.Data.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.Data.Score descending
             select RoomInstance.Value.Data).Take(amount);
        return rooms.ToList();
    }

    public List<RoomData> GetRoomsByCategory(int category, int amount = 50)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.Data.Category == category &&
             RoomInstance.Value.Data.UsersNow > 0 &&
             RoomInstance.Value.Data.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.Data.UsersNow descending
             select RoomInstance.Value.Data).Take(amount);
        return rooms.ToList();
    }

    public List<KeyValuePair<string, int>> GetPopularRoomTags()
    {
        var tags =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.Data.UsersNow >= 0 &&
             RoomInstance.Value.Data.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.Data.UsersNow descending
             orderby RoomInstance.Value.Data.Score descending
             select RoomInstance.Value.Data.Tags).Take(50);

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

    public List<RoomData> GetGroupRooms(int amount = 50)
    {
        var rooms =
            (from RoomInstance in this._rooms.ToList()
             where RoomInstance.Value.Data.Group != null &&
             RoomInstance.Value.Data.Access != RoomAccess.Invisible
             orderby RoomInstance.Value.Data.Score descending
             select RoomInstance.Value.Data).Take(amount);
        return rooms.ToList();
    }

    private readonly Stopwatch _roomCycleStopwatch;
    public void RoomCycleTask()
    {
        if (this._roomCycleStopwatch.ElapsedMilliseconds >= 500)
        {
            this._roomCycleStopwatch.Restart();

            var emptyRoomsCount = 0;
            foreach (var room in this._rooms.Values.ToList())
            {
                if (room.UserCount == 0)
                {
                    emptyRoomsCount++;
                }

                if (room.ProcessTask == null || room.ProcessTask.IsCompleted)
                {
                    room.ProcessTask = room.RunTask(() => room.ProcessRoom());

                    room.IsLagging = 0;
                }
                else
                {
                    room.IsLagging++;
                    if (room.IsLagging > 20)
                    {
                        ExceptionLogger.LogThreadException("Room lagging", "Room cycle task for room " + room.Id);

                        this.UnloadRoom(room);
                    }
                }
            }

            if (emptyRoomsCount >= 10)
            {
                WibboEnvironment.GetGame().GetRoomManager().UnloadEmptyRooms();
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
            room.Dispose();
        }
    }
}
