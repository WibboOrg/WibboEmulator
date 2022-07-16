namespace WibboEmulator.Game.Permissions
{
    class Permission
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public string Permissions { get; set; }

        public Permission(int id, int rank, string permission)
        {
            Id = id;
            Rank = rank;
            Permissions = permission;
        }
    }
}
