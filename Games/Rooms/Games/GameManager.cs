namespace WibboEmulator.Games.Rooms.Games;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Rooms.Games.Teams;

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

    public event EventHandler<TeamScoreChangedEventArgs> OnScoreChanged;
    public event EventHandler OnGameStart;
    public event EventHandler OnGameEnd;

    public GameManager(Room room)
    {
        this.TeamPoints = new int[5];

        this._redTeamItems = new Dictionary<int, Item>();
        this._blueTeamItems = new Dictionary<int, Item>();
        this._greenTeamItems = new Dictionary<int, Item>();
        this._yellowTeamItems = new Dictionary<int, Item>();
        this._roomInstance = room;
    }

    public Dictionary<int, Item> GetItems(TeamType team) => team switch
    {
        TeamType.Red => this._redTeamItems,
        TeamType.Green => this._greenTeamItems,
        TeamType.Blue => this._blueTeamItems,
        TeamType.Yellow => this._yellowTeamItems,
        _ => new Dictionary<int, Item>(),
    };

    public TeamType GetWinningTeam()
    {
        var nbTeam = 0;
        var maxPoints = 0;
        for (var i = 1; i < 5; ++i)
        {
            if (this.TeamPoints[i] == maxPoints)
            {
                nbTeam = 0;
            }

            if (this.TeamPoints[i] > maxPoints && this.TeamPoints[i] > 0)
            {
                maxPoints = this.TeamPoints[i];
                nbTeam = i;
            }
        }
        return (TeamType)nbTeam;
    }

    public void AddPointToTeam(TeamType team, RoomUser user) => this.AddPointToTeam(team, 1, user);

    public void AddPointToTeam(TeamType team, int points, RoomUser user)
    {
        var totalPoints = this.TeamPoints[(int)team] += points;
        if (totalPoints < 0)
        {
            totalPoints = 0;
        }

        if (totalPoints > 999)
        {
            totalPoints = 999;
        }

        this.TeamPoints[(int)team] = totalPoints;

        this.OnScoreChanged?.Invoke(null, new TeamScoreChangedEventArgs(totalPoints, team, user));

        foreach (var roomItem in this.GetFurniItems(team).Values)
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
        this.AddPointToTeam(TeamType.Blue, this.GetScoreForTeam(TeamType.Blue) * -1, null);
        this.AddPointToTeam(TeamType.Green, this.GetScoreForTeam(TeamType.Green) * -1, null);
        this.AddPointToTeam(TeamType.Red, this.GetScoreForTeam(TeamType.Red) * -1, null);
        this.AddPointToTeam(TeamType.Yellow, this.GetScoreForTeam(TeamType.Yellow) * -1, null);
    }

    private int GetScoreForTeam(TeamType team) => this.TeamPoints[(int)team];

    private Dictionary<int, Item> GetFurniItems(TeamType team) => team switch
    {
        TeamType.Red => this._redTeamItems,
        TeamType.Green => this._greenTeamItems,
        TeamType.Blue => this._blueTeamItems,
        TeamType.Yellow => this._yellowTeamItems,
        _ => new Dictionary<int, Item>(),
    };

    private static bool IsScoreItem(InteractionType type) => type switch
    {
        InteractionType.BANZAI_SCORE_BLUE or InteractionType.BANZAI_SCORE_RED or InteractionType.BANZAI_SCORE_YELLOW or InteractionType.BANZAI_SCORE_GREEN or InteractionType.FREEZE_BLUE_COUNTER or InteractionType.FREEZE_GREEN_COUNTER or InteractionType.FREEZE_RED_COUNTER or InteractionType.FREEZE_YELLOW_COUNTER => true,
        _ => false,
    };

    public void AddFurnitureToTeam(Item item, TeamType team)
    {
        switch (team)
        {
            case TeamType.Red:
                if (!this._redTeamItems.ContainsKey(item.Id))
                {
                    this._redTeamItems.Add(item.Id, item);
                }

                break;
            case TeamType.Green:
                if (!this._greenTeamItems.ContainsKey(item.Id))
                {
                    this._greenTeamItems.Add(item.Id, item);
                }

                break;
            case TeamType.Blue:
                if (!this._blueTeamItems.ContainsKey(item.Id))
                {
                    this._blueTeamItems.Add(item.Id, item);
                }

                break;
            case TeamType.Yellow:
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
            case TeamType.Red:
                _ = this._redTeamItems.Remove(item.Id);
                break;
            case TeamType.Green:
                _ = this._greenTeamItems.Remove(item.Id);
                break;
            case TeamType.Blue:
                _ = this._blueTeamItems.Remove(item.Id);
                break;
            case TeamType.Yellow:
                _ = this._yellowTeamItems.Remove(item.Id);
                break;
        }
    }

    public void UnlockGates()
    {
        foreach (var roomItem in this._redTeamItems.Values)
        {
            UnlockGate(roomItem);
        }

        foreach (var roomItem in this._greenTeamItems.Values)
        {
            UnlockGate(roomItem);
        }

        foreach (var roomItem in this._blueTeamItems.Values)
        {
            UnlockGate(roomItem);
        }

        foreach (var roomItem in this._yellowTeamItems.Values)
        {
            UnlockGate(roomItem);
        }
    }

    private static void LockGate(Item item)
    {
        switch (item.GetBaseItem().InteractionType)
        {
            case InteractionType.FREEZE_BLUE_GATE:
            case InteractionType.FREEZE_GREEN_GATE:
            case InteractionType.FREEZE_RED_GATE:
            case InteractionType.FREEZE_YELLOW_GATE:
            case InteractionType.BANZAI_GATE_BLUE:
            case InteractionType.BANZAI_GATE_GREEN:
            case InteractionType.BANZAI_GATE_RED:
            case InteractionType.BANZAI_GATE_YELLOW:
                //this.room.GetGameMap().UpdateGameMap(item, false);
                break;
        }
    }

    public void UpdateGatesTeamCounts()
    {
        foreach (var roomItem in this._redTeamItems.Values)
        {
            this.UpdateGateTeamCount(roomItem);
        }

        foreach (var roomItem in this._greenTeamItems.Values)
        {
            this.UpdateGateTeamCount(roomItem);
        }

        foreach (var roomItem in this._blueTeamItems.Values)
        {
            this.UpdateGateTeamCount(roomItem);
        }

        foreach (var roomItem in this._yellowTeamItems.Values)
        {
            this.UpdateGateTeamCount(roomItem);
        }
    }

    private void UpdateGateTeamCount(Item item)
    {
        switch (item.GetBaseItem().InteractionType)
        {
            case InteractionType.BANZAI_GATE_BLUE:
            case InteractionType.FREEZE_BLUE_GATE:
                item.ExtraData = this._roomInstance.TeamManager.BlueTeam.Count.ToString();
                item.UpdateState();
                break;
            case InteractionType.BANZAI_GATE_RED:
            case InteractionType.FREEZE_RED_GATE:
                item.ExtraData = this._roomInstance.TeamManager.RedTeam.Count.ToString();
                item.UpdateState();
                break;
            case InteractionType.BANZAI_GATE_GREEN:
            case InteractionType.FREEZE_GREEN_GATE:
                item.ExtraData = this._roomInstance.TeamManager.GreenTeam.Count.ToString();
                item.UpdateState();
                break;
            case InteractionType.BANZAI_GATE_YELLOW:
            case InteractionType.FREEZE_YELLOW_GATE:
                item.ExtraData = this._roomInstance.TeamManager.YellowTeam.Count.ToString();
                item.UpdateState();
                break;
        }
    }

    private static void UnlockGate(Item item)
    {
        switch (item.GetBaseItem().InteractionType)
        {
            case InteractionType.BANZAI_GATE_BLUE:
            case InteractionType.FREEZE_BLUE_GATE:
            case InteractionType.FREEZE_GREEN_GATE:
            case InteractionType.BANZAI_GATE_GREEN:
            case InteractionType.FREEZE_RED_GATE:
            case InteractionType.BANZAI_GATE_RED:
            case InteractionType.FREEZE_YELLOW_GATE:
            case InteractionType.BANZAI_GATE_YELLOW:
                //this.room.GetGameMap().UpdateGameMap(item, true);
                break;
        }
    }

    public void LockGates()
    {
        foreach (var roomItem in this._redTeamItems.Values)
        {
            LockGate(roomItem);
        }

        foreach (var roomItem in this._greenTeamItems.Values)
        {
            LockGate(roomItem);
        }

        foreach (var roomItem in this._blueTeamItems.Values)
        {
            LockGate(roomItem);
        }

        foreach (var roomItem in this._yellowTeamItems.Values)
        {
            LockGate(roomItem);
        }
    }

    public void StopGame()
    {
        this._roomInstance.BattleBanzai.BanzaiEnd();
        this._roomInstance.Freeze.StopGame();

        this.OnGameEnd?.Invoke(this, new());
    }

    public void StartGame()
    {
        this._roomInstance.BattleBanzai.BanzaiStart();
        this._roomInstance.Freeze.StartGame();

        this.OnGameStart?.Invoke(this, new());

        this._roomInstance.LastTimerReset = DateTime.Now;
    }

    public Room GetRoom() => this._roomInstance;

    public void Destroy()
    {
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
