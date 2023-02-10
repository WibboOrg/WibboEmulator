namespace WibboEmulator.Games.Permissions;

internal sealed class Permission
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public string Permissions { get; set; }

    public Permission(int id, int rank, string permission)
    {
        this.Id = id;
        this.Rank = rank;
        this.Permissions = permission;
    }
}
