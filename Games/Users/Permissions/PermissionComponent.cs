using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Games.GameClients.Permissions
{
    public sealed class PermissionComponent : IDisposable
    {
        private readonly User _userInstance;
        private readonly List<string> _permissions;
        private readonly List<string> _commands;

        public PermissionComponent(User user)
        {
            this._userInstance = user;
            _permissions = new List<string>();
            _commands = new List<string>();
        }

        public bool Init(IQueryAdapter dbClient)
        {
            if (_permissions.Count > 0)
                _permissions.Clear();

            if (_commands.Count > 0)
                _commands.Clear();

            return true;
        }

        public bool HasRight(string Right) => _permissions.Contains(Right);

        public bool HasCommand(string Command) => _commands.Contains(Command);

        public void Dispose()
        {
            _permissions.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
