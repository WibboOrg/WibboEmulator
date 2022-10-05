namespace WibboEmulator.Games.Permissions;

internal class PermissionCommand
{
    public string Input { get; private set; }
    public int MinRank { get; private set; }

    public PermissionCommand(string Command, int Rank)
    {
        this.Input = Command;
        this.MinRank = Rank;
    }
}
