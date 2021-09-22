using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Butterfly.HabboHotel.Animations
{
    public class AnimationManager
    {
        private const int MIN_USERS = 100; // mettre 80 co's
        private const int START_TIME = 20;
        private const int NOTIF_TIME = 2;
        private const int CLOSE_TIME = 1;

        private List<int> _roomId;
        private bool _started;
        private bool _skipCycle;
        private int _timer;
        private int _roomIdGame;

        private bool _isActivate;
        private bool _notif;
        private bool _forceDisabled;
        private int _RoomIdIndex;

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

            if (this._timer >= this.GetMinutes(START_TIME - NOTIF_TIME))
            {
                return false;
            }

            this._timer = 0;

            return true;
        }

        public string GetTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(this.GetMinutes(START_TIME) - this._timer);

            return time.Minutes + " minutes et " + time.Seconds + " secondes";
        }

        public void Init()
        {
            this._roomId.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id FROM rooms WHERE owner = 'WibboGame'");

                DataTable table = dbClient.GetTable();
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
                if (this._timer >= this.GetMinutes(CLOSE_TIME))
                {
                    Room Rooom = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(this._roomIdGame);

                    this._started = false;

                    if (Rooom != null)
                    {
                        Rooom.RoomData.State = 1;
                    }
                }
                return;
            }

            if (this._timer >= this.GetMinutes(START_TIME - NOTIF_TIME) && !this._notif)
            {
                this._notif = true;
                ButterflyEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifTopComposer("Notre prochaine animation va débuter dans 2 minutes - Jack & Daisy"), Core.Language.FRANCAIS);
            }

            if (this._timer >= this.GetMinutes(START_TIME))
            {
                this.StartGame();
            }
        }

        public void StartGame()
        {
            if (this._RoomIdIndex >= this._roomId.Count)
            {
                this._RoomIdIndex = 0;
                this._roomId = this._roomId.OrderBy(a => Guid.NewGuid()).ToList();
            }

            int RoomId = this._roomId[this._RoomIdIndex]; //ButterflyEnvironment.GetRandomNumber(0, this._roomId.Count - 1)
            this._RoomIdIndex++;

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
            if (room == null)
            {
                return;
            }

            this._timer = 0;
            this._started = true;
            this._notif = false;
            this._roomIdGame = RoomId;

            room.RoomData.State = 0;
            room.CloseFullRoom = true;

            string AlertMessage = "[i]Beep beep, c'est l'heure d'une animation ![/i]" +
            "[br][br]" +
            "Rejoins-nous chez [b]WibboGame[/b] pour un jeu qui s'intitule [b]" + room.RoomData.Name + "[/b]" +
            "[br][br]" +
            "Rends-toi dans l'appartement et tente de remporter un lot composé de [i]une ou plusieurs RareBox(s) et BadgeBox(s) ainsi qu'un point au TOP Gamer ![/i]" +
            "[br][br]" +
            "- Jack et Daisy";

            ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(1953042, "WibboGame", room.Id, string.Empty, "eventha", string.Format("JeuAuto EventHa: {0}", AlertMessage));
            ButterflyEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("gameauto", "Message d'animation", AlertMessage, "Je veux y jouer !", room.Id, ""));
        }

        public void OnCycle(Stopwatch moduleWatch)
        {
            this.Cycle();
            this.HandleFunctionReset(moduleWatch, "AnimationCycle");
        }

        private int GetMinutes(int minutes)
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
