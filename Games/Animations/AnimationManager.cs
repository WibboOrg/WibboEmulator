
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Rooms;
using System.Data;
using System.Diagnostics;

namespace WibboEmulator.Games.Animations
{
    public class AnimationManager
    {
        private const int MIN_USERS = 70;
        private const int START_TIME = 20;
        private const int NOTIF_TIME = 2;
        private const int CLOSE_TIME = 1;

        private List<int> _roomId;
        private string _gameOwner;
        private bool _started;
        private bool _skipCycle;
        private int _timer;
        private int _roomIdGame;

        private bool _isActivate;
        private bool _notif;
        private bool _forceDisabled;
        private int _roomIdIndex;

        public void OnUpdateUsersOnline(int usersOnline)
        {
            if (this._isActivate && usersOnline < MIN_USERS)
            {
                this._isActivate = false;
            }
            else if (!this._isActivate && usersOnline >= MIN_USERS)
            {
                this._isActivate = true;
            }
        }

        public bool ToggleForceDisabled()
        {
            this._forceDisabled = !this._forceDisabled;

            return this._forceDisabled;
        }

        public void ForceDisabled(bool Flag)
        {
            this._forceDisabled = Flag;
        }

        public AnimationManager()
        {
            this._roomId = new List<int>();
            this._started = false;
            this._timer = 0;
            this._roomIdGame = 0;
            this._isActivate = true;
            this._notif = false;
            this._skipCycle = false;
            this._forceDisabled = false;
        }

        public bool IsActivate()
        {
            return !this._forceDisabled && this._isActivate;
        }

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

            if (this._timer >= this.ToSeconds(START_TIME - NOTIF_TIME))
            {
                return false;
            }

            this._timer = 0;

            return true;
        }

        public string GetTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(this.ToSeconds(START_TIME) - this._timer);

            return time.Minutes + " minutes et " + time.Seconds + " secondes";
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._roomId.Clear();

            this._gameOwner = WibboEnvironment.GetSettings().GetData<string>("game.owner");

            DataTable table = RoomDao.GetAllIdByOwner(dbClient, this._gameOwner);
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

        private void Cycle()
        {
            if (!this._isActivate && !this._started)
            {
                return;
            }

            if (this._forceDisabled && !this._started)
            {
                return;
            }

            if (this._skipCycle)
            {
                this._timer++;
                this._skipCycle = false;
            }
            else
            {
                this._skipCycle = true;
            }

            if (this._started)
            {
                if (this._timer >= this.ToSeconds(CLOSE_TIME))
                {

                    this._started = false;

                    RoomData roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(this._roomIdGame);
                    if (roomData != null)
                    {
                        roomData.State = 1;
                    }
                }
                return;
            }

            if (this._timer >= this.ToSeconds(START_TIME - NOTIF_TIME) && !this._notif)
            {
                this._notif = true;
                WibboEnvironment.GetGame().GetClientManager().SendMessage(new NotifTopComposer("Notre prochaine animation aura lieu dans deux minutes ! (Jack & Daisy)"));
            }

            if (this._timer >= this.ToSeconds(START_TIME))
            {
                this.StartGame();
            }
        }

        public void StartGame(int forceRoomId = 0)
        {
            if (this._roomIdIndex >= this._roomId.Count)
            {
                this._roomIdIndex = 0;
                this._roomId = this._roomId.OrderBy(a => Guid.NewGuid()).ToList();
            }

            int roomId = this._roomId[this._roomIdIndex];

            if (forceRoomId != 0)
                roomId = forceRoomId;
            else
                this._roomIdIndex++;

            Room room = WibboEnvironment.GetGame().GetRoomManager().LoadRoom(roomId);
            if (room == null)
            {
                return;
            }

            this._timer = 0;
            this._started = true;
            this._notif = false;
            this._roomIdGame = roomId;

            room.RoomData.State = 0;
            room.CloseFullRoom = true;

            string alertMessage = "[center]" +
                "[size=large]🎮[b][u]ANIMATION AUTOMATIQUE[/u][/b] 🎮[/size]" +
                "[/center]" +
                "[center]" +
                "🤖 [b][i][color=696969]Une nouvelle animation automatique débute ![/color][/i][/b] 🤖" +
                "[/center][br]" +
                "➤ Rejoins-nous chez [color=696969][b]WibboGame[/b][/color] dans le jeu [color=696969][u][b]" + room.RoomData.Name + "[/b][/u][/color] pour une animation automatisée ![br]" +
                "➤ Rejoins nous et tente de remporter des [b]RareBoxs[/b] ainsi qu'un [b]point au [u]TOP Gamer[/u][/b] ![br][br]" +
                "[center][img]https://cdn.wibbo.org/uploads/1659791208.png[/img]  - Jack et Daisy, [b][u][color=696969]Animateurs robotisés[/color][/u][/b] 🤖  -  [img]https://cdn.wibbo.org/uploads/1659791188.png[/img][/center]";

            WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(1953042, this._gameOwner, room.Id, string.Empty, "eventha", string.Format("JeuAuto EventHa: {0}", alertMessage));

            WibboEnvironment.GetGame().GetClientManager().SendMessage(new NotifAlertComposer(
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
            this.Cycle();
            this.HandleFunctionReset(moduleWatch, "AnimationCycle");
        }

        private int ToSeconds(int minutes)
        {
            return (minutes * 60);
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
}
