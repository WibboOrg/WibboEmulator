using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Games;

namespace Butterfly.Utilities.Events
{
    public class TeamScoreChangedArgs : EventArgs
    {
        public readonly int Points;
        public readonly TeamType Team;
        public readonly RoomUser user;

        public TeamScoreChangedArgs(int points, TeamType team, RoomUser user)
        {
            this.Points = points;
            this.Team = team;
            this.user = user;
        }
    }
}
