namespace WibboEmulator.Game.Rooms.Games
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
            if (t.Equals(TeamType.BLUE))
            {
                return this.BlueTeam.Count < 5;
            }

            if (t.Equals(TeamType.RED))
            {
                return this.RedTeam.Count < 5;
            }

            if (t.Equals(TeamType.YELLOW))
            {
                return this.YellowTeam.Count < 5;
            }

            if (t.Equals(TeamType.GREEN))
            {
                return this.GreenTeam.Count < 5;
            }

            return false;
        }

        public void AddUser(RoomUser user)
        {
            if (user.Team.Equals(TeamType.BLUE))
            {
                this.BlueTeam.Add(user);
            }
            else if (user.Team.Equals(TeamType.RED))
            {
                this.RedTeam.Add(user);
            }
            else if (user.Team.Equals(TeamType.YELLOW))
            {
                this.YellowTeam.Add(user);
            }
            else if (user.Team.Equals(TeamType.GREEN))
            {
                this.GreenTeam.Add(user);
            }
        }

        public void OnUserLeave(RoomUser user)
        {
            if (user.Team.Equals(TeamType.BLUE))
            {
                this.BlueTeam.Remove(user);
            }
            else if (user.Team.Equals(TeamType.RED))
            {
                this.RedTeam.Remove(user);
            }
            else if (user.Team.Equals(TeamType.YELLOW))
            {
                this.YellowTeam.Remove(user);
            }
            else if (user.Team.Equals(TeamType.GREEN))
            {
                this.GreenTeam.Remove(user);
            }
        }
    }
}
