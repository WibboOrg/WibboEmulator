namespace Butterfly.HabboHotel.Permissions
{
    class PermissionCommand
    {
        public string Command { get; private set; }
        public int MinRank { get; private set; }
        public PermissionCommand(string command, int minrank)
        {
            Command = command;
            MinRank = minrank;
        }
    }
}
