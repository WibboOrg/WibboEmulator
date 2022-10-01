namespace WibboEmulator.Games.Permissions
{
    class PermissionCommand
    {
        public string Input { get; private set; }
        public int MinRank { get; private set; }

        public PermissionCommand(string Command, int Rank)
        {
            Input = Command;
            MinRank = Rank;
        }
    }
}
