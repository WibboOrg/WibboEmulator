namespace WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

public class TeamScoreChangedEventArgs(int points, TeamType team, RoomUser user) : EventArgs
{
    public int Points { get; private set; } = points;
    public TeamType Team { get; private set; } = team;
    public RoomUser User { get; private set; } = user;
}
