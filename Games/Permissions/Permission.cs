namespace WibboEmulator.Games.Permissions;

internal sealed class Permission(int id, int rank, string permission)
{
    public int Id { get; set; } = id;
    public int Rank { get; set; } = rank;
    public string Permissions { get; set; } = permission;
}
