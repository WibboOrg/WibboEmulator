using WibboEmulator.Games.Items;
using WibboEmulator.Utilities.Events;

namespace WibboEmulator.Games.Rooms.Games
{
    public class GameManager
    {
        public int[] TeamPoints { get; set; }

        private Dictionary<int, Item> _redTeamItems;
        private Dictionary<int, Item> _blueTeamItems;
        private Dictionary<int, Item> _greenTeamItems;
        private Dictionary<int, Item> _yellowTeamItems;
        private Room _roomInstance;

        public int[] Points
        {
            get => this.TeamPoints;
            set => this.TeamPoints = value;
        }

        public event TeamScoreChangedDelegate OnScoreChanged;

        public event RoomEventDelegate OnGameStart;

        public event RoomEventDelegate OnGameEnd;

        public GameManager(Room room)
        {
            this.TeamPoints = new int[5];

            this._redTeamItems = new Dictionary<int, Item>();
            this._blueTeamItems = new Dictionary<int, Item>();
            this._greenTeamItems = new Dictionary<int, Item>();
            this._yellowTeamItems = new Dictionary<int, Item>();
            this._roomInstance = room;
        }

        public Dictionary<int, Item> GetItems(TeamType team)
        {
            switch (team)
            {
                case TeamType.RED:
                    return this._redTeamItems;
                case TeamType.GREEN:
                    return this._greenTeamItems;
                case TeamType.BLUE:
                    return this._blueTeamItems;
                case TeamType.YELLOW:
                    return this._yellowTeamItems;
                default:
                    return new Dictionary<int, Item>();
            }
        }

        public TeamType GetWinningTeam()
        {
            int NbTeam = 0;
            int MaxPoints = 0;
            for (int i = 1; i < 5; ++i)
            {
                if (this.TeamPoints[i] == MaxPoints)
                {
                    NbTeam = 0;
                }

                if (this.TeamPoints[i] > MaxPoints && this.TeamPoints[i] > 0)
                {
                    MaxPoints = this.TeamPoints[i];
                    NbTeam = i;
                }
            }
            return (TeamType)NbTeam;
        }

        public void AddPointToTeam(TeamType team, RoomUser user) => this.AddPointToTeam(team, 1, user);

        public void AddPointToTeam(TeamType team, int points, RoomUser user)
        {
            int points1 = this.TeamPoints[(int)team] += points;
            if (points1 < 0)
            {
                points1 = 0;
            }

            if (points1 > 999)
            {
                points1 = 999;
            }

            this.TeamPoints[(int)team] = points1;

            if (this.OnScoreChanged != null)
            {
                this.OnScoreChanged(null, new TeamScoreChangedArgs(points1, team, user));
            }

            foreach (Item roomItem in this.GetFurniItems(team).Values)
            {
                if (IsScoreItem(roomItem.GetBaseItem().InteractionType))
                {
                    roomItem.ExtraData = this.TeamPoints[(int)team].ToString();
                    roomItem.UpdateState();
                }
            }
        }

        public void Reset()
        {
            this.AddPointToTeam(TeamType.BLUE, this.GetScoreForTeam(TeamType.BLUE) * -1, null);
            this.AddPointToTeam(TeamType.GREEN, this.GetScoreForTeam(TeamType.GREEN) * -1, null);
            this.AddPointToTeam(TeamType.RED, this.GetScoreForTeam(TeamType.RED) * -1, null);
            this.AddPointToTeam(TeamType.YELLOW, this.GetScoreForTeam(TeamType.YELLOW) * -1, null);
        }

        private int GetScoreForTeam(TeamType team) => this.TeamPoints[(int)team];

        private Dictionary<int, Item> GetFurniItems(TeamType team)
        {
            switch (team)
            {
                case TeamType.RED:
                    return this._redTeamItems;
                case TeamType.GREEN:
                    return this._greenTeamItems;
                case TeamType.BLUE:
                    return this._blueTeamItems;
                case TeamType.YELLOW:
                    return this._yellowTeamItems;
                default:
                    return new Dictionary<int, Item>();
            }
        }

        private static bool isSoccerGoal(InteractionType type)
        {
            if (type != InteractionType.footballgoalblue && type != InteractionType.FOOTBALLGOALGREEN && type != InteractionType.FOOTBALLGOALRED && type != InteractionType.FOOTBALLGOALYELLOW)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsScoreItem(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.BANZAISCOREBLUE:
                case InteractionType.BANZAISCORERED:
                case InteractionType.BANZAISCOREYELLOW:
                case InteractionType.BANZAISCOREGREEN:

                case InteractionType.FREEZEBLUECOUNTER:
                case InteractionType.FREEZEGREENCOUNTER:
                case InteractionType.FREEZEREDCOUNTER:
                case InteractionType.FREEZEYELLOWCOUNTER:
                    return true;
            }
            return false;

        }

        public void AddFurnitureToTeam(Item item, TeamType team)
        {
            switch (team)
            {
                case TeamType.RED:
                    if (!this._redTeamItems.ContainsKey(item.Id))
                    {
                        this._redTeamItems.Add(item.Id, item);
                    }

                    break;
                case TeamType.GREEN:
                    if (!this._greenTeamItems.ContainsKey(item.Id))
                    {
                        this._greenTeamItems.Add(item.Id, item);
                    }

                    break;
                case TeamType.BLUE:
                    if (!this._blueTeamItems.ContainsKey(item.Id))
                    {
                        this._blueTeamItems.Add(item.Id, item);
                    }

                    break;
                case TeamType.YELLOW:
                    if (!this._yellowTeamItems.ContainsKey(item.Id))
                    {
                        this._yellowTeamItems.Add(item.Id, item);
                    }

                    break;
            }
        }

        public void RemoveFurnitureFromTeam(Item item, TeamType team)
        {
            switch (team)
            {
                case TeamType.RED:
                    this._redTeamItems.Remove(item.Id);
                    break;
                case TeamType.GREEN:
                    this._greenTeamItems.Remove(item.Id);
                    break;
                case TeamType.BLUE:
                    this._blueTeamItems.Remove(item.Id);
                    break;
                case TeamType.YELLOW:
                    this._yellowTeamItems.Remove(item.Id);
                    break;
            }
        }

        public void UnlockGates()
        {
            foreach (Item roomItem in this._redTeamItems.Values)
            {
                this.UnlockGate(roomItem);
            }

            foreach (Item roomItem in this._greenTeamItems.Values)
            {
                this.UnlockGate(roomItem);
            }

            foreach (Item roomItem in this._blueTeamItems.Values)
            {
                this.UnlockGate(roomItem);
            }

            foreach (Item roomItem in this._yellowTeamItems.Values)
            {
                this.UnlockGate(roomItem);
            }
        }

        private void LockGate(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FREEZEBLUEGATE:
                case InteractionType.FREEZEGREENGATE:
                case InteractionType.FREEZEREDGATE:
                case InteractionType.FREEZEYELLOWGATE:
                case InteractionType.BANZAIGATEBLUE:
                case InteractionType.BANZAIGATEGREEN:
                case InteractionType.BANZAIGATERED:
                case InteractionType.BANZAIGATEYELLOW:
                    //this.room.GetGameMap().UpdateGameMap(item, false);
                    break;
            }
        }

        public void UpdateGatesTeamCounts()
        {
            foreach (Item roomItem in this._redTeamItems.Values)
            {
                this.UpdateGateTeamCount(roomItem);
            }

            foreach (Item roomItem in this._greenTeamItems.Values)
            {
                this.UpdateGateTeamCount(roomItem);
            }

            foreach (Item roomItem in this._blueTeamItems.Values)
            {
                this.UpdateGateTeamCount(roomItem);
            }

            foreach (Item roomItem in this._yellowTeamItems.Values)
            {
                this.UpdateGateTeamCount(roomItem);
            }
        }

        private void UpdateGateTeamCount(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.BANZAIGATEBLUE:
                case InteractionType.FREEZEBLUEGATE:
                    item.ExtraData = this._roomInstance.GetTeamManager().BlueTeam.Count.ToString();
                    item.UpdateState();
                    break;
                case InteractionType.BANZAIGATERED:
                case InteractionType.FREEZEREDGATE:
                    item.ExtraData = this._roomInstance.GetTeamManager().RedTeam.Count.ToString();
                    item.UpdateState();
                    break;
                case InteractionType.BANZAIGATEGREEN:
                case InteractionType.FREEZEGREENGATE:
                    item.ExtraData = this._roomInstance.GetTeamManager().GreenTeam.Count.ToString();
                    item.UpdateState();
                    break;
                case InteractionType.BANZAIGATEYELLOW:
                case InteractionType.FREEZEYELLOWGATE:
                    item.ExtraData = this._roomInstance.GetTeamManager().YellowTeam.Count.ToString();
                    item.UpdateState();
                    break;
            }
        }

        private void UnlockGate(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.BANZAIGATEBLUE:
                case InteractionType.FREEZEBLUEGATE:
                case InteractionType.FREEZEGREENGATE:
                case InteractionType.BANZAIGATEGREEN:
                case InteractionType.FREEZEREDGATE:
                case InteractionType.BANZAIGATERED:
                case InteractionType.FREEZEYELLOWGATE:
                case InteractionType.BANZAIGATEYELLOW:
                    //this.room.GetGameMap().UpdateGameMap(item, true);
                    break;
            }
        }

        public void LockGates()
        {
            foreach (Item roomItem in this._redTeamItems.Values)
            {
                this.LockGate(roomItem);
            }

            foreach (Item roomItem in this._greenTeamItems.Values)
            {
                this.LockGate(roomItem);
            }

            foreach (Item roomItem in this._blueTeamItems.Values)
            {
                this.LockGate(roomItem);
            }

            foreach (Item roomItem in this._yellowTeamItems.Values)
            {
                this.LockGate(roomItem);
            }
        }

        public void StopGame()
        {
            this._roomInstance.GetBanzai().BanzaiEnd();
            this._roomInstance.GetFreeze().StopGame();

            if (this.OnGameEnd != null)
            {
                this.OnGameEnd(null, null);
            }
        }

        public void StartGame()
        {
            this._roomInstance.GetBanzai().BanzaiStart();
            this._roomInstance.GetFreeze().StartGame();

            this.OnGameStart?.Invoke(null, null);

            this._roomInstance.lastTimerReset = DateTime.Now;
        }

        public Room GetRoom() => this._roomInstance;

        public void Destroy()
        {
            Array.Clear(this.TeamPoints, 0, this.TeamPoints.Length);
            this._redTeamItems.Clear();
            this._blueTeamItems.Clear();
            this._greenTeamItems.Clear();
            this._yellowTeamItems.Clear();
            this.TeamPoints = null;
            this.OnScoreChanged = null;
            this.OnGameStart = null;
            this.OnGameEnd = null;
            this._redTeamItems = null;
            this._blueTeamItems = null;
            this._greenTeamItems = null;
            this._yellowTeamItems = null;
            this._roomInstance = null;
        }
    }
}
