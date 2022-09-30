using WibboEmulator.Core;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;

namespace WibboEmulator.Game.Rooms
{
    public class RoomManager
    {
        private readonly ConcurrentDictionary<int, Room> _rooms;
        private readonly Dictionary<string, RoomModel> _roomModels;
        private readonly ConcurrentDictionary<int, RoomData> _roomsData;

        public int Count => this._rooms.Count;

        public RoomManager()
        {
            this._rooms = new ConcurrentDictionary<int, Room>();
            this._roomModels = new Dictionary<string, RoomModel>();
            this._roomsData = new ConcurrentDictionary<int, RoomData>();

            this.roomCycleStopwatch = new Stopwatch();
            this.roomCycleStopwatch.Start();
        }

        private static RoomModel GetCustomData(int roomID)
        {
            DataRow row;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                row = RoomModelCustomDao.GetOne(dbClient, roomID);
            }

            if (row == null)
            {
                throw new Exception("The custom room model for room " + roomID + " was not found");
            }

            return new RoomModel(roomID.ToString(), Convert.ToInt32(row["door_x"]), Convert.ToInt32(row["door_y"]), (double)row["door_z"], Convert.ToInt32(row["door_dir"]), (string)row["heightmap"], Convert.ToInt32(row["wall_height"]));
        }

        public RoomModel GetModel(string Model, int RoomID)
        {
            if (Model == "model_custom")
            {
                return GetCustomData(RoomID);
            }

            if (this._roomModels.ContainsKey(Model))
            {
                return this._roomModels[Model];
            }
            else
            {
                return null;
            }
        }

        public RoomData GenerateNullableRoomData(int RoomId)
        {
            if (this.GenerateRoomData(RoomId) != null)
            {
                return this.GenerateRoomData(RoomId);
            }

            RoomData roomData = new RoomData();
            roomData.FillNull(RoomId);
            return roomData;
        }

        public RoomData GenerateRoomData(int RoomId)
        {
            if (this.TryGetRoom(RoomId, out Room Room))
            {
                return Room.RoomData;
            }

            if (this.TryGetRoomData(RoomId, out RoomData roomData))
            {
                return roomData;
            }

            DataRow Row = null;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Row = RoomDao.GetOne(dbClient, RoomId);
            }

            if (Row == null)
            {
                return null;
            }

            roomData = new RoomData();
            roomData.Fill(Row);

            if (!this._roomsData.ContainsKey(RoomId))
            {
                this._roomsData.TryAdd(RoomId, roomData);
            }

            return roomData;
        }

        public Room LoadRoom(int Id)
        {
            if (this.TryGetRoom(Id, out Room Room))
            {
                return Room;
            }

            RoomData Data = this.GenerateRoomData(Id);
            if (Data == null)
            {
                return null;
            }

            Room = new Room(Data);

            if (!this._rooms.ContainsKey(Room.Id))
            {
                this._rooms.TryAdd(Room.Id, Room);
            }

            if (this._roomsData.ContainsKey(Room.Id))
            {
                this._roomsData.TryRemove(Room.Id, out Data);
            }

            return Room;
        }

        public void RoomDataRemove(int Id)
        {
            if (this._roomsData.ContainsKey(Id))
            {
                this._roomsData.TryRemove(Id, out RoomData Data);
            }
        }

        public bool TryGetRoom(int RoomId, out Room Room)
        {
            return this._rooms.TryGetValue(RoomId, out Room);
        }

        public bool TryGetRoomModels(string model, out RoomModel roomModel)
        {
            return this._roomModels.TryGetValue(model, out roomModel);
        }

        public bool TryGetRoomData(int RoomId, out RoomData RoomData)
        {
            return this._roomsData.TryGetValue(RoomId, out RoomData);
        }

        public RoomData FetchRoomData(int roomID, DataRow dRow)
        {
            if(this.TryGetRoom(roomID, out Room room))
            {
                return room.RoomData;
            }
            else
            {
                RoomData data = new RoomData();
                data.Fill(dRow);
                return data;
            }
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._roomModels.Clear();

            DataTable roomMoodelData = RoomModelDao.GetAll(dbClient);
            if (roomMoodelData == null)
            {
                return;
            }

            foreach (DataRow dataRow in roomMoodelData.Rows)
            {
                string str = (string)dataRow["id"];
                this._roomModels.Add(str, new RoomModel(str, Convert.ToInt32(dataRow["door_x"]), Convert.ToInt32(dataRow["door_y"]), (double)dataRow["door_z"], Convert.ToInt32(dataRow["door_dir"]), (string)dataRow["heightmap"], 0));
            }
        }

        public void OnCycle(Stopwatch moduleWatch)
        {
            this.RoomCycleTask();
            this.HandleFunctionReset(moduleWatch, "RoomCycleTask");
        }

        private void HandleFunctionReset(Stopwatch watch, string methodName)
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

        public List<RoomData> SearchGroupRooms(string Query)
        {
            IEnumerable<RoomData> InstanceMatches =
                (from RoomInstance in this._rooms.ToList()
                 where RoomInstance.Value.RoomData.UsersNow >= 0 &&
                 RoomInstance.Value.RoomData.State != 3 &&
                 RoomInstance.Value.RoomData.Group != null &&
                 (RoomInstance.Value.RoomData.OwnerName.StartsWith(Query) ||
                 RoomInstance.Value.RoomData.Tags.Contains(Query) ||
                 RoomInstance.Value.RoomData.Name.Contains(Query))
                 orderby RoomInstance.Value.RoomData.UsersNow descending
                 select RoomInstance.Value.RoomData).Take(50);
            return InstanceMatches.ToList();
        }

        public List<RoomData> SearchTaggedRooms(string Query)
        {
            IEnumerable<RoomData> InstanceMatches =
                (from RoomInstance in this._rooms.ToList()
                 where RoomInstance.Value.RoomData.UsersNow >= 0 &&
                 RoomInstance.Value.RoomData.State != 3 &&
                 (RoomInstance.Value.RoomData.Tags.Contains(Query))
                 orderby RoomInstance.Value.RoomData.UsersNow descending
                 select RoomInstance.Value.RoomData).Take(50);
            return InstanceMatches.ToList();
        }

        public List<RoomData> GetPopularRooms(int category, int Amount = 50, Language Langue = Language.FRANCAIS)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._rooms.ToList()
                 where RoomInstance.Value != null && RoomInstance.Value.RoomData != null &&
                 RoomInstance.Value.RoomData.UsersNow > 0 &&
                 (category == -1 || RoomInstance.Value.RoomData.Category == category) &&
                 RoomInstance.Value.RoomData.State != 3 && RoomInstance.Value.RoomData.Langue == Langue
                 orderby RoomInstance.Value.RoomData.Score descending
                 orderby RoomInstance.Value.RoomData.UsersNow descending
                 select RoomInstance.Value.RoomData).Take(Amount);
            return rooms.ToList();
        }

        public List<RoomData> GetRecommendedRooms(int Amount = 50, int CurrentRoomId = 0)
        {
            IEnumerable<RoomData> Rooms =
                (from RoomInstance in this._rooms.ToList()
                 where RoomInstance.Value.RoomData.UsersNow >= 0 &&
                 RoomInstance.Value.RoomData.Score >= 0 &&
                 RoomInstance.Value.RoomData.State != 3 &&
                 RoomInstance.Value.RoomData.Id != CurrentRoomId
                 orderby RoomInstance.Value.RoomData.Score descending
                 orderby RoomInstance.Value.RoomData.UsersNow descending
                 select RoomInstance.Value.RoomData).Take(Amount);
            return Rooms.ToList();
        }

        public List<RoomData> GetPopularRatedRooms(int Amount = 50)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._rooms.ToList()
                 where RoomInstance.Value.RoomData.State != 3
                 orderby RoomInstance.Value.RoomData.Score descending
                 select RoomInstance.Value.RoomData).Take(Amount);
            return rooms.ToList();
        }

        public List<RoomData> GetRoomsByCategory(int Category, int Amount = 50)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._rooms.ToList()
                 where RoomInstance.Value.RoomData.Category == Category &&
                 RoomInstance.Value.RoomData.UsersNow > 0 &&
                 RoomInstance.Value.RoomData.State != 3
                 orderby RoomInstance.Value.RoomData.UsersNow descending
                 select RoomInstance.Value.RoomData).Take(Amount);
            return rooms.ToList();
        }

        public List<KeyValuePair<string, int>> GetPopularRoomTags()
        {
            IEnumerable<List<string>> Tags =
                (from RoomInstance in this._rooms.ToList()
                 where RoomInstance.Value.RoomData.UsersNow >= 0 &&
                 RoomInstance.Value.RoomData.State != 3
                 orderby RoomInstance.Value.RoomData.UsersNow descending
                 orderby RoomInstance.Value.RoomData.Score descending
                 select RoomInstance.Value.RoomData.Tags).Take(50);

            Dictionary<string, int> TagValues = new Dictionary<string, int>();

            foreach (List<string> TagList in Tags)
            {
                foreach (string Tag in TagList)
                {
                    if (!TagValues.ContainsKey(Tag))
                    {
                        TagValues.Add(Tag, 1);
                    }
                    else
                    {
                        TagValues[Tag]++;
                    }
                }
            }

            List<KeyValuePair<string, int>> SortedTags = new List<KeyValuePair<string, int>>(TagValues);
            SortedTags.Sort((FirstPair, NextPair) =>
            {
                return FirstPair.Value.CompareTo(NextPair.Value);
            });

            SortedTags.Reverse();
            return SortedTags;
        }

        public List<RoomData> GetGroupRooms(int Amount = 50)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._rooms.ToList()
                 where RoomInstance.Value.RoomData.Group != null &&
                 RoomInstance.Value.RoomData.State != 3
                 orderby RoomInstance.Value.RoomData.Score descending
                 select RoomInstance.Value.RoomData).Take(Amount);
            return rooms.ToList();
        }

        public Room TryGetRandomLoadedRoom()
        {
            IEnumerable<Room> room =
                (from RoomInstance in this._rooms.ToList()
                 where (RoomInstance.Value.RoomData.UsersNow > 0 &&
                 RoomInstance.Value.RoomData.State == 0 &&
                 RoomInstance.Value.RoomData.UsersNow < RoomInstance.Value.RoomData.UsersMax)
                 orderby RoomInstance.Value.RoomData.UsersNow descending
                 select RoomInstance.Value).Take(1);

            if (room.Count() > 0)
            {
                return room.First();
            }
            else
            {
                return null;
            }
        }

        private readonly Stopwatch roomCycleStopwatch;
        public void RoomCycleTask()
        {
            if (this.roomCycleStopwatch.ElapsedMilliseconds >= 500)
            {
                this.roomCycleStopwatch.Restart();
                foreach (Room room in this._rooms.Values.ToList())
                {
                    if (!(room.ProcessTask == null || room.ProcessTask.IsCompleted) && !room.isCycling && !room.Disposed)
                    {
                        room.ProcessTask = new Task(room.ProcessRoom);
                        room.ProcessTask.Start();

                        room.IsLagging = 0;
                    }
                    else
                    {
                        room.IsLagging++;
                        if (room.IsLagging > 20)
                        {
                            this.UnloadRoom(room);
                        }
                    }
                }
            }
        }

        public void RemoveAllRooms()
        {
            int count = this._rooms.Count;
            int num = 0;

            foreach (Room Room in this._rooms.Values.ToList())
            {
                if (Room == null)
                {
                    continue;
                }

                WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);
                Console.Clear();
                Console.WriteLine("<<- SERVER SHUTDOWN ->> ROOM ITEM SAVE: " + string.Format("{0:0.##}", (num / (double)count * 100.0)) + "%");
                num++;
            }

            Console.WriteLine("Done disposing rooms!");
        }

        public void UnloadRoom(Room Room)
        {
            if (Room == null)
            {
                return;
            }

            if (this._rooms.TryRemove(Room.Id, out Room room))
            {
                Room.Dispose();
            }
        }
    }
}
