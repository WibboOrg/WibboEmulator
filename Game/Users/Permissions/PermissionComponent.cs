using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Game.Users.Permissions
{
    public sealed class PermissionComponent : IDisposable
    {
        private readonly List<string> _permissions;
        private readonly List<string> _commands;

        public PermissionComponent(User u)
        {
            _permissions = new List<string>();
            _commands = new List<string>();
        }

        public bool Init(IQueryAdapter dbClient, User user)
        {
            if (_permissions.Count > 0)
                _permissions.Clear();

            if (_commands.Count > 0)
                _commands.Clear();

            return true;
        }

        public bool HasRight(string Right)
        {
            return _permissions.Contains(Right);
        }

        public bool HasCommand(string Command)
        {
            return _commands.Contains(Command);
        }

        public void Dispose()
        {
            _permissions.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
