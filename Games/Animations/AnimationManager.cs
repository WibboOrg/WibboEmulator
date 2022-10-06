namespace WibboEmulator.Games.Animations;

using System.Data;
using System.Diagnostics;
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Interfaces;

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

    public void OnUpdateUsersOnline(int usersOnline)
    {
        var minUsers = WibboEnvironment.GetSettings().GetData<int>("autogame.min.users");

        if (this._isActivate && usersOnline < minUsers)
        {
            this._isActivate = false;
        }
        else if (!this._isActivate && usersOnline >= minUsers)
        {
            this._animationTime = DateTime.Now;
            this._isActivate = true;
        }
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

        return true;
    }

    public string GetTime()
    {
        var time = this._animationTime - DateTime.Now + this._startTime;

        return time.Minutes + " minutes et " + time.Seconds + " secondes";
    }

    public void Init(IQueryAdapter dbClient)
    {
        this._roomId.Clear();

        var gameOwner = WibboEnvironment.GetSettings().GetData<string>("autogame.owner");
        var table = RoomDao.GetAllIdByOwner(dbClient, gameOwner);
        if (table == null)
        {
            return;
        }

        foreach (DataRow dataRow in table.Rows)
        {
            if (this._roomId.Contains(Convert.ToInt32(dataRow["id"])))
            {
                continue;
            }

            this._roomId.Add(Convert.ToInt32(dataRow["id"]));
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
                        roomData.State = 1;
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

        var roomId = this._roomId[this._roomIdIndex];

        if (forceRoomId != 0)
        {
            roomId = forceRoomId;
        }
        else
        {
            this._roomIdIndex++;
        }

        var room = WibboEnvironment.GetGame().GetRoomManager().LoadRoom(roomId);
        if (room == null)
        {
            return;
        }

        this._animationTime = DateTime.Now;
        this._started = true;
        this._notif = false;
        this._roomIdGame = roomId;

        room.RoomData.State = 0;
        room.CloseFullRoom = true;

        var alertMessage = "[center]" +
            "[size=large]🎮[b][u]ANIMATION AUTOMATIQUE[/u][/b] 🎮[/size]" +
            "[/center]" +
            "[center]" +
            "🤖 [b][i][color=696969]Une nouvelle animation automatique débute ![/color][/i][/b] 🤖" +
            "[/center][br]" +
            "➤ Rejoins-nous chez [color=696969][b]WibboGame[/b][/color] dans le jeu [color=696969][u][b]" + room.RoomData.Name + "[/b][/u][/color] pour une animation automatisée ![br]" +
            "➤ Rejoins nous et tente de remporter des [b]RareBoxs[/b] ainsi qu'un [b]point au [u]TOP Gamer[/u][/b] ![br][br]" +
            "[center][img]https://cdn.wibbo.org/uploads/1659791208.png[/img]  - Jack et Daisy, [b][u][color=696969]Animateurs robotisés[/color][/u][/b] 🤖  -  [img]https://cdn.wibbo.org/uploads/1659791188.png[/img][/center]";
        var gameOwner = WibboEnvironment.GetSettings().GetData<string>("autogame.owner");

        WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(1953042, gameOwner, room.Id, string.Empty, "eventha", string.Format("JeuAuto EventHa: {0}", alertMessage));

        WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifAlertComposer(
            "gameauto", // image
            "Message d'animation", // title
            alertMessage, // string_>alert
            "Je veux y jouer !", // button
            room.Id, //guide
            ""
        ));
    }

    public void OnCycle(Stopwatch moduleWatch)
    {
        this.AnimationCycle();
        this.HandleFunctionReset(moduleWatch, "AnimationCycle");
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

}
