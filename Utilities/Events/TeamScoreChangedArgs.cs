namespace WibboEmulator.Utilities.Events;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

public class TeamScoreChangedArgs : EventArgs
{
    public int Points { get; private set; }
    public TeamType Team { get; private set; }
    public RoomUser User { get; private set; }

    public TeamScoreChangedArgs(int points, TeamType team, RoomUser user)
    {
        this.Points = points;
        this.Team = team;
        this.User = user;
    }
}
