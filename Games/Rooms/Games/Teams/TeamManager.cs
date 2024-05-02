namespace WibboEmulator.Games.Rooms.Games.Teams;
public class TeamManager
{
    public List<RoomUser> BlueTeam { get; set; }
    public List<RoomUser> RedTeam { get; set; }
    public List<RoomUser> YellowTeam { get; set; }
    public List<RoomUser> GreenTeam { get; set; }

    public TeamManager()
    {
        this.BlueTeam = [];
        this.RedTeam = [];
        this.GreenTeam = [];
        this.YellowTeam = [];
    }

    public List<RoomUser> AllPlayers
    {
        get
        {
            var players = new List<RoomUser>();

            players.AddRange(this.BlueTeam);
            players.AddRange(this.RedTeam);
            players.AddRange(this.GreenTeam);
            players.AddRange(this.YellowTeam);

            return players;
        }
    }

    public bool CanEnterOnTeam(TeamType t)
    {
        if (t.Equals(TeamType.Blue))
        {
            return this.BlueTeam.Count < 5;
        }

        if (t.Equals(TeamType.Red))
        {
            return this.RedTeam.Count < 5;
        }

        if (t.Equals(TeamType.Yellow))
        {
            return this.YellowTeam.Count < 5;
        }

        if (t.Equals(TeamType.Green))
        {
            return this.GreenTeam.Count < 5;
        }

        return false;
    }

    public void AddUser(RoomUser user)
    {
        if (user.Team.Equals(TeamType.Blue))
        {
            this.BlueTeam.Add(user);
        }
        else if (user.Team.Equals(TeamType.Red))
        {
            this.RedTeam.Add(user);
        }
        else if (user.Team.Equals(TeamType.Yellow))
        {
            this.YellowTeam.Add(user);
        }
        else if (user.Team.Equals(TeamType.Green))
        {
            this.GreenTeam.Add(user);
        }
    }

    public void OnUserLeave(RoomUser user)
    {
        if (user.Team.Equals(TeamType.Blue))
        {
            _ = this.BlueTeam.Remove(user);
        }
        else if (user.Team.Equals(TeamType.Red))
        {
            _ = this.RedTeam.Remove(user);
        }
        else if (user.Team.Equals(TeamType.Yellow))
        {
            _ = this.YellowTeam.Remove(user);
        }
        else if (user.Team.Equals(TeamType.Green))
        {
            _ = this.GreenTeam.Remove(user);
        }
    }
}
