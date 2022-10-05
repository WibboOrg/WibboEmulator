namespace WibboEmulator.Games.Users.Permissions;
using WibboEmulator.Database.Interfaces;

public sealed class PermissionComponent : IDisposable
{
    private readonly User _userInstance;
    private readonly List<string> _permissions;
    private readonly List<string> _commands;

    public PermissionComponent(User user)
    {
        this._userInstance = user;
        this._permissions = new List<string>();
        this._commands = new List<string>();
    }

    public bool Init(IQueryAdapter dbClient)
    {
        if (this._permissions.Count > 0)
        {
            this._permissions.Clear();
        }

        if (this._commands.Count > 0)
        {
            this._commands.Clear();
        }

        return true;
    }

    public bool HasRight(string right) => this._permissions.Contains(right);

    public bool HasCommand(string command) => this._commands.Contains(command);

    public void Dispose()
    {
        this._permissions.Clear();
        GC.SuppressFinalize(this);
    }
}
