namespace WibboEmulator.Games.Animations;

using System.Data;
using System.Diagnostics;
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Core.Language;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Rooms;

public static class AnimationManager
{
    private static readonly TimeSpan StartTime = TimeSpan.FromMinutes(20);
    private static readonly TimeSpan NotifTime = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan CloseTime = TimeSpan.FromMinutes(1);

    private static DateTime _animationTime = DateTime.Now;
    private static List<int> _roomId = [];
    private static bool _started;
    private static int _roomIdGame;

    private static bool _isActivate = true;
    private static bool _notif;
    private static bool _forceDisabled;
    private static int _roomIdIndex;

    public static void OnUpdateUsersOnline(int usersOnline)
    {
        var minUsers = SettingsManager.GetData<int>("autogame.min.users");

        _isActivate = usersOnline >= minUsers;
    }

    public static bool ToggleForceDisabled
    {
        get
        {
            _forceDisabled = !_forceDisabled;

            if (!_forceDisabled)
            {
                _animationTime = DateTime.Now;
            }

            return _forceDisabled;
        }
    }

    public static void ForceDisabled(bool flag)
    {
        _forceDisabled = flag;

        if (!_forceDisabled)
        {
            _animationTime = DateTime.Now;
        }
    }

    public static bool IsActivate => !_forceDisabled && _isActivate;

    public static bool AllowAnimation
    {
        get
        {
            if (!IsActivate)
            {
                return true;
            }

            if (_started)
            {
                return false;
            }

            var time = DateTime.Now - _animationTime;

            if (time >= StartTime - NotifTime)
            {
                return false;
            }

            _animationTime = DateTime.Now;

            return true;
        }
    }

    public static string Time
    {
        get
        {
            var time = _animationTime - DateTime.Now + StartTime;

            return $"{time.Minutes} minutes et {time.Seconds} secondes";
        }
    }

    public static void Initialize(IDbConnection dbClient)
    {
        AnimationCycleStopwatch.Start();

        _roomId.Clear();

        var gameOwner = SettingsManager.GetData<string>("autogame.owner");

        var roomIdList = RoomDao.GetAllIdByOwner(dbClient, gameOwner);
        if (roomIdList.Count == 0)
        {
            return;
        }

        foreach (var id in roomIdList)
        {
            if (_roomId.Contains(id))
            {
                continue;
            }

            _roomId.Add(id);
        }

        if (_roomId.Count == 0)
        {
            _forceDisabled = true;
        }
    }

    private static readonly Stopwatch AnimationCycleStopwatch = new();
    private static void AnimationCycle()
    {
        if (AnimationCycleStopwatch.ElapsedMilliseconds >= 1000)
        {
            AnimationCycleStopwatch.Restart();

            if (!_isActivate && !_started)
            {
                return;
            }

            if (_forceDisabled && !_started)
            {
                return;
            }

            var time = DateTime.Now - _animationTime;

            if (_started)
            {
                if (time >= CloseTime)
                {
                    _started = false;

                    var roomData = RoomManager.GenerateRoomData(_roomIdGame);
                    if (roomData != null)
                    {
                        roomData.Access = RoomAccess.Doorbell;
                    }
                }
                return;
            }

            if (time >= StartTime - NotifTime && !_notif)
            {
                _notif = true;
                GameClientManager.SendMessage(new NotifTopComposer("Notre prochaine animation aura lieu dans deux minutes ! (Jack & Daisy)"));
            }

            if (time >= StartTime)
            {
                StartGame();
            }
        }
    }

    public static void StartGame(int forceRoomId = 0)
    {
        if (_roomIdIndex >= _roomId.Count)
        {
            _roomIdIndex = 0;
            _roomId = [.. _roomId.OrderBy(a => Guid.NewGuid())];
        }

        var roomId = forceRoomId != 0 ? forceRoomId : _roomId[_roomIdIndex++];

        var room = RoomManager.LoadRoom(roomId);
        if (room == null)
        {
            return;
        }

        _animationTime = DateTime.Now;
        _started = true;
        _notif = false;
        _roomIdGame = roomId;

        room.RoomData.Access = RoomAccess.Open;
        room.CloseFullRoom = true;

        var alertMessage = string.Format(LanguageManager.TryGetValue("autogame.alert", Language.French), room.RoomData.Name);

        var gameOwner = SettingsManager.GetData<string>("autogame.owner");

        ModerationManager.LogStaffEntry(1953042, gameOwner, room.Id, string.Empty, "eventha", string.Format("JeuAuto EventHa: {0}", alertMessage));

        GameClientManager.SendMessage(new NotifAlertComposer("gameauto", "Message d'animation", alertMessage, "Je veux y jouer !", room.Id, ""));
    }

    public static void OnCycle(Stopwatch moduleWatch)
    {
        AnimationCycle();
        HandleFunctionReset(moduleWatch, "AnimationCycle");
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
}
