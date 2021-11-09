namespace Butterfly.HabboHotel.Permissions
{
    class Permission
    {
        public int Id { get; set; }
        public int PermissionRank { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }

        public Permission(int id, int Rank, string Name,  string description)
        {
            Id = id;
            PermissionRank = Rank;
            PermissionName = Name;
            Description = description;
        }
    }
}
