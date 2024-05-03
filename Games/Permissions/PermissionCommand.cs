namespace WibboEmulator.Games.Permissions;

internal sealed class PermissionCommand(string command, int rank)
{
    public string Input { get; private set; } = command;
    public int MinRank { get; private set; } = rank;
}
