namespace WibboEmulator.Game.Groups
{
    public class GroupMember
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Look { get; set; }

        public GroupMember(int Id, string Username, string Look)
        {
            this.Id = Id;
            this.Username = Username;
            this.Look = Look;
        }
    }
}