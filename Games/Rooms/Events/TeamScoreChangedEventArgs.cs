namespace WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

public class TeamScoreChangedEventArgs : EventArgs
{
    public int Points { get; private set; }
    public TeamType Team { get; private set; }
    public RoomUser User { get; private set; }

    public TeamScoreChangedEventArgs(int points, TeamType team, RoomUser user)
    {
        this.Points = points;
        this.Team = team;
        this.User = user;
    }
}
