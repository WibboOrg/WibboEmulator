namespace WibboEmulator.Games.Animations;

using System.Data;
using System.Diagnostics;
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Rooms;

public class AnimationManager
{
    private readonly TimeSpan _startTime = TimeSpan.FromMinutes(20);
    private readonly TimeSpan _notifTime = TimeSpan.FromMinutes(2);
    private readonly TimeSpan _closeTime = TimeSpan.FromMinutes(1);

    private DateTime _animationTime;
    private List<int> _roomId;
    private bool _started;
    private int _roomIdGame;

    private bool _isActivate;
    private bool _notif;
    private bool _forceDisabled;
    private int _roomIdIndex;

    public AnimationManager()
    {
        this._roomId = new List<int>();
        this._started = false;
        this._roomIdGame = 0;
        this._isActivate = true;
        this._notif = false;
        this._forceDisabled = false;
        this._animationTime = DateTime.Now;

        this._animationCycleStopwatch = new();
        this._animationCycleStopwatch.Start();
    }

    public void OnUpdateUsersOnline(int usersOnline)
    {
        var minUsers = WibboEnvironment.GetSettings().GetData<int>("autogame.min.users");

        this._isActivate = usersOnline >= minUsers;
    }

    public bool ToggleForceDisabled()
    {
        this._forceDisabled = !this._forceDisabled;

        if (!this._forceDisabled)
        {
            this._animationTime = DateTime.Now;
        }

        return this._forceDisabled;
    }

    public void ForceDisabled(bool flag)
    {
        this._forceDisabled = flag;

        if (!this._forceDisabled)
        {
            this._animationTime = DateTime.Now;
        }
    }

    public bool IsActivate() => !this._forceDisabled && this._isActivate;

    public bool AllowAnimation()
    {
        if (!this.IsActivate())
        {
            return true;
        }

        if (this._started)
        {
            return false;
        }

        var time = DateTime.Now - this._animationTime;

        if (time >= this._startTime - this._notifTime)
        {
            return false;
        }

        this._animationTime = DateTime.Now;

        return true;
    }

    public string GetTime()
    {
        var time = this._animationTime - DateTime.Now + this._startTime;

        return $"{time.Minutes} minutes et {time.Seconds} secondes";
    }

    public void Init(IDbConnection dbClient)
    {
        this._roomId.Clear();

        var gameOwner = WibboEnvironment.GetSettings().GetData<string>("autogame.owner");

        var roomIdList = RoomDao.GetAllIdByOwner(dbClient, gameOwner);
        if (roomIdList.Count == 0)
        {
            return;
        }

        foreach (var id in roomIdList)
        {
            if (this._roomId.Contains(id))
            {
                continue;
            }

            this._roomId.Add(id);
        }

        if (this._roomId.Count == 0)
        {
            this._forceDisabled = true;
        }
    }

    private readonly Stopwatch _animationCycleStopwatch;
    private void AnimationCycle()
    {
        if (this._animationCycleStopwatch.ElapsedMilliseconds >= 1000)
        {
            this._animationCycleStopwatch.Restart();

            if (!this._isActivate && !this._started)
            {
                return;
            }

            if (this._forceDisabled && !this._started)
            {
                return;
            }

            var time = DateTime.Now - this._animationTime;

            if (this._started)
            {
                if (time >= this._closeTime)
                {
                    this._started = false;

                    var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(this._roomIdGame);
                    if (roomData != null)
                    {
                        roomData.Access = RoomAccess.Doorbell;
                    }
                }
                return;
            }

            if (time >= this._startTime - this._notifTime && !this._notif)
            {
                this._notif = true;
                WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifTopComposer("Notre prochaine animation aura lieu dans deux minutes ! (Jack & Daisy)"));
            }

            if (time >= this._startTime)
            {
                this.StartGame();
            }
        }
    }

    public void StartGame(int forceRoomId = 0)
    {
        if (this._roomIdIndex >= this._roomId.Count)
        {
            this._roomIdIndex = 0;
            this._roomId = this._roomId.OrderBy(a => Guid.NewGuid()).ToList();
        }

        var roomId = forceRoomId != 0 ? forceRoomId : this._roomId[this._roomIdIndex++];

        var room = WibboEnvironment.GetGame().GetRoomManager().LoadRoom(roomId);
        if (room == null)
        {
            return;
        }

        this._animationTime = DateTime.Now;
        this._started = true;
        this._notif = false;
        this._roomIdGame = roomId;

        room.RoomData.Access = RoomAccess.Open;
        room.CloseFullRoom = true;

        var alertMessage = string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("autogame.alert", Language.French), room.RoomData.Name);

        var gameOwner = WibboEnvironment.GetSettings().GetData<string>("autogame.owner");

        ModerationManager.LogStaffEntry(1953042, gameOwner, room.Id, string.Empty, "eventha", string.Format("JeuAuto EventHa: {0}", alertMessage));

        WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifAlertComposer(
            "gameauto",
            "Message d'animation",
            alertMessage,
            "Je veux y jouer !",
            room.Id,
            ""
        ));
    }

    public void OnCycle(Stopwatch moduleWatch)
    {
        this.AnimationCycle();
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
