namespace WibboEmulator.Games.Permissions;

internal sealed class PermissionCommand
{
    public string Input { get; private set; }
    public int MinRank { get; private set; }

    public PermissionCommand(string command, int rank)
    {
        this.Input = command;
        this.MinRank = rank;
    }
}
