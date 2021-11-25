using System.Collections.Generic;

namespace Butterfly.Game.Rooms.Games
{
    public class TeamManager
    {
        public List<RoomUser> BlueTeam { get; set; }
        public List<RoomUser> RedTeam { get; set; }
        public List<RoomUser> YellowTeam { get; set; }
        public List<RoomUser> GreenTeam { get; set; }

        public TeamManager()
        {
            this.BlueTeam = new List<RoomUser>();
            this.RedTeam = new List<RoomUser>();
            this.GreenTeam = new List<RoomUser>();
            this.YellowTeam = new List<RoomUser>();
        }

        public List<RoomUser> GetAllPlayer()
        {
            List<RoomUser> Players = new List<RoomUser>();

            Players.AddRange(this.BlueTeam);
            Players.AddRange(this.RedTeam);
            Players.AddRange(this.GreenTeam);
            Players.AddRange(this.YellowTeam);

            return Players;
        }

        public bool CanEnterOnTeam(TeamType t)
        {
            if (t.Equals(TeamType.blue))
            {
                return this.BlueTeam.Count < 5;
            }

            if (t.Equals(TeamType.red))
            {
                return this.RedTeam.Count < 5;
            }

            if (t.Equals(TeamType.yellow))
            {
                return this.YellowTeam.Count < 5;
            }

            if (t.Equals(TeamType.green))
            {
                return this.GreenTeam.Count < 5;
            }

            return false;
        }

        public void AddUser(RoomUser user)
        {
            if (user.Team.Equals(TeamType.blue))
            {
                this.BlueTeam.Add(user);
            }
            else if (user.Team.Equals(TeamType.red))
            {
                this.RedTeam.Add(user);
            }
            else if (user.Team.Equals(TeamType.yellow))
            {
                this.YellowTeam.Add(user);
            }
            else if (user.Team.Equals(TeamType.green))
            {
                this.GreenTeam.Add(user);
            }
        }

        public void OnUserLeave(RoomUser user)
        {
            if (user.Team.Equals(TeamType.blue))
            {
                this.BlueTeam.Remove(user);
            }
            else if (user.Team.Equals(TeamType.red))
            {
                this.RedTeam.Remove(user);
            }
            else if (user.Team.Equals(TeamType.yellow))
            {
                this.YellowTeam.Remove(user);
            }
            else if (user.Team.Equals(TeamType.green))
            {
                this.GreenTeam.Remove(user);
            }
        }
    }
}
