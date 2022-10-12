namespace WibboEmulator.Games.Rooms.Games;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Utilities.Events;

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
        var points1 = this.TeamPoints[(int)team] += points;
        if (points1 < 0)
        {
            points1 = 0;
        }

        if (points1 > 999)
        {
            points1 = 999;
        }

        this.TeamPoints[(int)team] = points1;

        this.OnScoreChanged?.Invoke(null, new TeamScoreChangedEventArgs(points1, team, user));

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
        InteractionType.BANZAISCOREBLUE or InteractionType.BANZAISCORERED or InteractionType.BANZAISCOREYELLOW or InteractionType.BANZAISCOREGREEN or InteractionType.FREEZEBLUECOUNTER or InteractionType.FREEZEGREENCOUNTER or InteractionType.FREEZEREDCOUNTER or InteractionType.FREEZEYELLOWCOUNTER => true,
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

    private static void UnlockGate(Item item)
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
        this._roomInstance.GetBanzai().BanzaiEnd();
        this._roomInstance.GetFreeze().StopGame();

        this.OnGameEnd?.Invoke(this, new());
    }

    public void StartGame()
    {
        this._roomInstance.GetBanzai().BanzaiStart();
        this._roomInstance.GetFreeze().StartGame();

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
